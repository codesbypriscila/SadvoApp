using AutoMapper;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Entities.Dirigente;
using SADVO.Domain.Entities.Elector;
using SADVO.Domain.Interfaces;
using System.Linq.Expressions;

namespace SADVO.Application.Services
{
    public class EleccionService : IEleccionService
    {
        private readonly IGenericRepository<Eleccion> _eleccionRepo;
        private readonly IGenericRepository<PuestoElectivo> _puestoRepo;
        private readonly IGenericRepository<PartidoPolitico> _partidoRepo;
        private readonly IGenericRepository<CandidatoPuesto> _candidatoRepo;
        private readonly IGenericRepository<Voto> _votoRepo;
        private readonly IGenericRepository<Candidato> _candidatoRepoGeneric;
        private readonly IMapper _mapper;

        public EleccionService(
            IGenericRepository<Eleccion> eleccionRepo,
            IGenericRepository<PuestoElectivo> puestoRepo,
            IGenericRepository<PartidoPolitico> partidoRepo,
            IGenericRepository<CandidatoPuesto> candidatoRepo,
            IGenericRepository<Voto> votoRepo,
            IGenericRepository<Candidato> candidatoRepoGeneric,
            IMapper mapper)
        {
            _eleccionRepo = eleccionRepo;
            _puestoRepo = puestoRepo;
            _partidoRepo = partidoRepo;
            _candidatoRepo = candidatoRepo;
            _votoRepo = votoRepo;
            _candidatoRepoGeneric = candidatoRepoGeneric;
            _mapper = mapper;
        }

        public async Task<bool> HayEleccionActivaAsync()
        {
            var hoy = DateTime.Today;
            return await _eleccionRepo.ExistsAsync(e =>
                e.Activa &&
                e.Fecha.Date == hoy &&
                !e.Finalizada
            );
        }

        public async Task<IEnumerable<EleccionDto>> GetAllAsync()
        {
            var list = await _eleccionRepo.GetAllAsync();
            return list
                .OrderByDescending(e => e.Activa)
                .ThenByDescending(e => e.Fecha)
                .Select(e =>
                {
                    var dto = _mapper.Map<EleccionDto>(e);
                    dto.CantidadPartidosPoliticos = e.Candidatos.Select(c => c.PartidoPoliticoId).Distinct().Count();
                    dto.CantidadPuestosDisputados = e.Candidatos.Select(c => c.PuestoElectivoId).Distinct().Count();
                    return dto;
                });
        }

        public async Task<EleccionDto?> GetByIdAsync(int id)
        {
            var entity = await _eleccionRepo.GetByIdAsync(id);
            return entity != null ? _mapper.Map<EleccionDto>(entity) : null;
        }

        public async Task CreateAsync(EleccionDto dto)
        {
            var puestos = await _puestoRepo.FindAsync(p => p.Activo);
            if (!puestos.Any())
                throw new Exception("No hay puestos electivos activos.");

            var partidos = await _partidoRepo.FindAsync(p => p.Activo);
            if (partidos.Count() < 2)
                throw new Exception("No hay suficientes partidos políticos para realizar una elección.");

            foreach (var partido in partidos)
            {
                var candidatos = await _candidatoRepo.FindAsync(c => c.PartidoPoliticoId == partido.Id);
                var faltantes = puestos.Where(p => !candidatos.Any(c => c.PuestoElectivoId == p.Id)).ToList();
                if (faltantes.Any())
                {
                    var nombres = string.Join(", ", faltantes.Select(p => p.Nombre));
                    throw new Exception($"El partido político {partido.Nombre} ({partido.Siglas}) no tiene candidatos para los puestos: {nombres}");
                }
            }

            var entity = _mapper.Map<Eleccion>(dto);
            await _eleccionRepo.AddAsync(entity);
        }

        public Task UpdateAsync(EleccionDto dto)
        {
            var entity = _mapper.Map<Eleccion>(dto);
            _eleccionRepo.UpdateAsync(entity);
            return Task.CompletedTask;
        }

        public async Task FinalizarAsync(int id)
        {
            var eleccion = await _eleccionRepo.GetByIdAsync(id);
            if (eleccion != null && eleccion.Activa)
            {
                eleccion.Activa = false;
                eleccion.Finalizada = true;
                _eleccionRepo.UpdateAsync(eleccion);
            }
        }

        public async Task<ResultadoEleccionDto?> ObtenerResultadosAsync(int eleccionId)
        {
            var eleccion = await _eleccionRepo.GetByIdAsync(eleccionId);
            if (eleccion == null || !eleccion.Finalizada) return null;

            var votos = await _votoRepo.FindAsync(v => v.EleccionId == eleccionId);
            var resultado = new ResultadoEleccionDto { NombreEleccion = eleccion.Nombre };

            var puestosAgrupados = votos.GroupBy(v => v.PuestoElectivoId);

            foreach (var grupoPuesto in puestosAgrupados)
            {
                var puesto = await _puestoRepo.GetByIdAsync(grupoPuesto.Key);
                if (puesto == null) continue;

                var resumen = new ResumenPuestoDto { NombrePuesto = puesto.Nombre };
                var totalVotos = grupoPuesto.Count();

                var candidatosAgrupados = grupoPuesto
                    .GroupBy(v => v.CandidatoId)
                    .OrderByDescending(g => g.Count());

                foreach (var grupoCandidato in candidatosAgrupados)
                {
                    var candidato = await _candidatoRepoGeneric.GetByIdAsync(grupoCandidato.Key);
                    if (candidato == null) continue;

                    var votosCandidato = grupoCandidato.Count();
                    var porcentaje = totalVotos == 0 ? 0 : Math.Round((double)votosCandidato * 100 / totalVotos, 2);

                    resumen.Candidatos.Add(new ResumenCandidatoDto
                    {
                        NombreCandidato = $"{candidato.Nombre} {candidato.Apellido}",
                        Partido = candidato.PartidoPolitico.Nombre,
                        Votos = votosCandidato,
                        Porcentaje = porcentaje
                    });
                }

                resultado.Puestos.Add(resumen);
            }

            return resultado;
        }
    }
}