using AutoMapper;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Entities.Dirigente;
using SADVO.Domain.Entities.Enums;
using SADVO.Domain.Interfaces;

namespace SADVO.Application.Services
{
    public class CandidatoPuestoService : ICandidatoPuestoService
    {
        private readonly IGenericRepository<CandidatoPuesto> _repository;
        private readonly IGenericRepository<Candidato> _candidatoRepo;
        private readonly IGenericRepository<PuestoElectivo> _puestoRepo;
        private readonly IGenericRepository<AlianzaPolitica> _alianzaRepo;
        private readonly IMapper _mapper;

        public CandidatoPuestoService(IGenericRepository<CandidatoPuesto> repository,
                                      IGenericRepository<Candidato> candidatoRepo,
                                      IGenericRepository<PuestoElectivo> puestoRepo,
                                      IGenericRepository<AlianzaPolitica> alianzaRepo,
                                      IMapper mapper)
        {
            _repository = repository;
            _candidatoRepo = candidatoRepo;
            _puestoRepo = puestoRepo;
            _alianzaRepo = alianzaRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CandidatoPuestoDto>> GetAllAsync(int partidoId)
        {
            var asignaciones = await _repository.GetAllAsync();
            var candidatos = await _candidatoRepo.GetAllAsync();
            var puestos = await _puestoRepo.GetAllAsync();

            var resultado = asignaciones
                .Where(x => x.PartidoPoliticoId == partidoId)
                .Select(cp => new CandidatoPuestoDto
                {
                    Id = cp.Id,
                    CandidatoId = cp.CandidatoId,
                    PuestoElectivoId = cp.PuestoElectivoId,
                    PartidoPoliticoId = cp.PartidoPoliticoId,
                    NombreCandidato = candidatos.FirstOrDefault(c => c.Id == cp.CandidatoId) is { } candidato
                        ? $"{candidato.Nombre} {candidato.Apellido}"
                        : "Candidato no encontrado",
                    ApellidoCandidato = candidatos.FirstOrDefault(c => c.Id == cp.CandidatoId)?.Apellido ?? "N/A",
                    NombrePuesto = puestos.FirstOrDefault(p => p.Id == cp.PuestoElectivoId)?.Nombre ?? "N/A"
                });

            return resultado.ToList();
        }



        public async Task CreateAsync(CandidatoPuestoDto dto)
        {
            var todos = await _repository.GetAllAsync();

            var asignaciones = todos
                .Where(x => x.CandidatoId == dto.CandidatoId && x.PartidoPoliticoId == dto.PartidoPoliticoId)
                .ToList();

            if (asignaciones.Any())
                throw new InvalidOperationException("Este candidato ya está asignado a un puesto dentro del partido.");

            var candidato = await _candidatoRepo.GetByIdAsync(dto.CandidatoId);
            var asignacionesDelCandidato = todos.Where(x => x.CandidatoId == dto.CandidatoId).ToList();
            var puestoElegido = await _puestoRepo.GetByIdAsync(dto.PuestoElectivoId);

            foreach (var asig in asignacionesDelCandidato)
            {
                if (asig.PartidoPoliticoId != dto.PartidoPoliticoId &&
                    asig.PuestoElectivoId != dto.PuestoElectivoId)
                {
                    throw new InvalidOperationException("Este candidato en su partido de origen aspira a un puesto diferente al seleccionado.");
                }
            }

            var entidad = _mapper.Map<CandidatoPuesto>(dto);
            await _repository.AddAsync(entidad);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
                _repository.RemoveAsync(entity);
        }

        public async Task<List<CandidatoDto>> ObtenerCandidatosDisponiblesAsync(int partidoId, int puestoId)
        {
            var alianzasTodas = await _alianzaRepo.GetAllAsync();
            var aliados = alianzasTodas.Where(x =>
                (x.PartidoSolicitanteId == partidoId || x.PartidoReceptorId == partidoId) &&
                x.Estado == EstadoAlianza.Aceptada).ToList();

            var aliadosIds = aliados.SelectMany(x =>
                new[] { x.PartidoSolicitanteId, x.PartidoReceptorId }).Distinct().ToList();

            var todosCandidatos = await _candidatoRepo.GetAllAsync();
            var candidatos = todosCandidatos.Where(x => x.Activo).ToList();
            var asignaciones = await _repository.GetAllAsync();

            var resultado = new List<CandidatoDto>();

            foreach (var c in candidatos)
            {
                bool esDelPartido = c.PartidoPoliticoId == partidoId;
                bool esAliado = aliadosIds.Contains(c.PartidoPoliticoId);
                var asignacionesCandidato = asignaciones.Where(a => a.CandidatoId == c.Id).ToList();

                if (esDelPartido)
                {
                    if (!asignacionesCandidato.Any(a => a.PartidoPoliticoId == partidoId))
                        resultado.Add(_mapper.Map<CandidatoDto>(c));
                }
                else if (esAliado)
                {
                    foreach (var asig in asignacionesCandidato)
                    {
                        if (asig.PuestoElectivoId == puestoId)
                        {
                            resultado.Add(_mapper.Map<CandidatoDto>(c));
                            break;
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<List<PuestoElectivoDto>> ObtenerPuestosDisponiblesAsync(int partidoId)
        {
            var todosAsignaciones = await _repository.GetAllAsync();
            var asignaciones = todosAsignaciones
                .Where(x => x.PartidoPoliticoId == partidoId)
                .ToList();

            var puestosAsignados = asignaciones.Select(a => a.PuestoElectivoId).ToList();

            var todosPuestos = await _puestoRepo.GetAllAsync();
            var puestos = todosPuestos
                .Where(x => x.Activo && !puestosAsignados.Contains(x.Id))
                .ToList();

            return _mapper.Map<List<PuestoElectivoDto>>(puestos);
        }
    }
}
