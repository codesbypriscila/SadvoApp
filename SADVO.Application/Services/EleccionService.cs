using AutoMapper;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Entities.Dirigente;
using SADVO.Domain.Entities.Elector;
using SADVO.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            var list = await _eleccionRepo.GetAllIncludingAsync(e => e.Candidatos);

            return list
                .OrderByDescending(e => e.Activa)
                .ThenByDescending(e => e.Fecha)
                .Select(e =>
                {
                    var dto = _mapper.Map<EleccionDto>(e);
                    dto.CantidadPartidosPoliticos = e.Candidatos?.Select(c => c.PartidoPoliticoId).Distinct().Count() ?? 0;
                    dto.CantidadPuestosDisputados = e.Candidatos?.Select(c => c.PuestoElectivoId).Distinct().Count() ?? 0;
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
            var puestos = (await _puestoRepo.FindAsync(p => p.Activo)).ToList();
            if (!puestos.Any())
                throw new Exception("Debe haber al menos un puesto electivo activo.");

            var partidos = (await _partidoRepo.FindAsync(p => p.Activo)).ToList();
            if (partidos.Count < 2)
                throw new Exception("No hay suficientes partidos políticos para realizar una elección.");

            var entity = _mapper.Map<Eleccion>(dto);
            entity.Activa = true;
            entity.Finalizada = false;
            entity.Candidatos = new List<CandidatoPuesto>();

            foreach (var partido in partidos)
            {
                var puestosSinCandidato = new List<string>();

                foreach (var puesto in puestos)
                {
                    var candidatosPuestos = await _candidatoRepo.FindAsync(cp =>
                        cp.PartidoPoliticoId == partido.Id &&
                        cp.PuestoElectivoId == puesto.Id &&
                        cp.Candidato.Activo);

                    var candidatoPuesto = candidatosPuestos.FirstOrDefault();

                    if (candidatoPuesto == null)
                    {
                        puestosSinCandidato.Add(puesto.Nombre);
                    }
                    else
                    {
                        entity.Candidatos.Add(new CandidatoPuesto
                        {
                            Eleccion = entity,
                            CandidatoId = candidatoPuesto.CandidatoId,
                            PuestoElectivoId = puesto.Id,
                            PartidoPoliticoId = partido.Id
                        });
                    }
                }

                if (puestosSinCandidato.Any())
                {
                    var nombres = string.Join(", ", puestosSinCandidato);
                    throw new Exception($"El partido político {partido.Nombre} ({partido.Siglas}) no tiene candidatos para los puestos: {nombres}");
                }
            }

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
                    var candidato = await _candidatoRepoGeneric
                        .GetFirstOrDefaultAsync(
                            c => c.Id == grupoCandidato.Key,
                            include: q => q.Include(c => c.PartidoPolitico)
                        );

                    if (candidato == null) continue;

                    var votosCandidato = grupoCandidato.Count();
                    var porcentaje = totalVotos == 0 ? 0 : Math.Round((double)votosCandidato * 100 / totalVotos, 2);

                    resumen.Candidatos.Add(new ResumenCandidatoDto
                    {
                        NombreCandidato = $"{candidato.Nombre} {candidato.Apellido}",
                        Partido = candidato.PartidoPolitico?.Nombre ?? "Desconocido",
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