using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Entities.Dirigente;
using SADVO.Domain.Entities.Enums;
using SADVO.Domain.Interfaces;


namespace SADVO.Application.Services
{
    public class AlianzaPoliticaService : IAlianzaPoliticaService
    {
        private readonly IGenericRepository<AlianzaPolitica> _repo;
        private readonly IGenericRepository<PartidoPolitico> _partidoRepo;
        private readonly IEleccionService _eleccionService;
        private readonly IMapper _mapper;

        public AlianzaPoliticaService(
            IGenericRepository<AlianzaPolitica> repo,
            IGenericRepository<PartidoPolitico> partidoRepo,
            IEleccionService eleccionService,
            IMapper mapper)
        {
            _repo = repo;
            _partidoRepo = partidoRepo;
            _eleccionService = eleccionService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AlianzaPoliticaDto>> ObtenerSolicitudesPendientesAsync(int partidoId)
        {
            var alianzas = await _repo.FindAsync(
                a => a.PartidoReceptorId == partidoId && a.Estado == EstadoAlianza.EnEspera,
                include: q => q.Include(a => a.PartidoSolicitante)
            );

            return _mapper.Map<IEnumerable<AlianzaPoliticaDto>>(alianzas);
        }


        public async Task<IEnumerable<AlianzaPoliticaDto>> ObtenerSolicitudesEnviadasAsync(int partidoId)
        {
            var alianzas = await _repo.FindAsync(
                a => a.PartidoSolicitanteId == partidoId,
                include: q => q.Include(a => a.PartidoReceptor)
            );

            return _mapper.Map<IEnumerable<AlianzaPoliticaDto>>(alianzas);
        }


        public async Task<IEnumerable<AlianzaPoliticaDto>> ObtenerAlianzasActivasAsync(int partidoId)
        {
            var alianzas = await _repo.FindAsync(
                a => a.Estado == EstadoAlianza.Aceptada &&
                     (a.PartidoSolicitanteId == partidoId || a.PartidoReceptorId == partidoId),
                include: q => q.Include(a => a.PartidoSolicitante)
                               .Include(a => a.PartidoReceptor)
            );

            return _mapper.Map<IEnumerable<AlianzaPoliticaDto>>(alianzas);
        }

        public async Task CrearSolicitudAsync(int solicitanteId, int receptorId)
        {
            if (await _eleccionService.HayEleccionActivaAsync())
                throw new InvalidOperationException("No se puede crear una alianza con una elección activa.");

            var yaExiste = await _repo.ExistsAsync(a =>
                ((a.PartidoSolicitanteId == solicitanteId && a.PartidoReceptorId == receptorId) ||
                 (a.PartidoSolicitanteId == receptorId && a.PartidoReceptorId == solicitanteId))
                && a.Estado == EstadoAlianza.EnEspera);

            if (yaExiste)
                throw new InvalidOperationException("Ya existe una solicitud pendiente entre estos partidos.");

            var nueva = new AlianzaPolitica
            {
                PartidoSolicitanteId = solicitanteId,
                PartidoReceptorId = receptorId,
                Estado = EstadoAlianza.EnEspera,
                FechaSolicitud = DateTime.Now
            };

            await _repo.AddAsync(nueva);
        }

        public async Task AceptarSolicitudAsync(int id)
        {
            var alianza = await _repo.GetByIdAsync(id) ?? throw new Exception("No encontrada");

            if (await _eleccionService.HayEleccionActivaAsync())
                throw new InvalidOperationException("No se puede aceptar una alianza con una elección activa.");

            alianza.Estado = EstadoAlianza.Aceptada;
            alianza.FechaRespuesta = DateTime.Now;
            _repo.UpdateAsync(alianza);
        }

        public async Task RechazarSolicitudAsync(int id)
        {
            var alianza = await _repo.GetByIdAsync(id) ?? throw new Exception("No encontrada");

            if (await _eleccionService.HayEleccionActivaAsync())
                throw new InvalidOperationException("No se puede rechazar una alianza con una elección activa.");

            alianza.Estado = EstadoAlianza.Rechazada;
            alianza.FechaRespuesta = DateTime.Now;
            _repo.UpdateAsync(alianza);
        }

        public async Task EliminarSolicitudAsync(int id)
        {
            var alianza = await _repo.GetByIdAsync(id) ?? throw new Exception("No encontrada");

            if (await _eleccionService.HayEleccionActivaAsync())
                throw new InvalidOperationException("No se puede eliminar una alianza con una elección activa.");

            _repo.RemoveAsync(alianza);
        }

        public async Task<AlianzaPoliticaDto?> ObtenerPorIdAsync(int id)
        {
            var resultado = await _repo.FindAsync(
                a => a.Id == id,
                include: q => q
                    .Include(a => a.PartidoSolicitante)
                    .Include(a => a.PartidoReceptor)
            );

            return _mapper.Map<AlianzaPoliticaDto>(resultado.FirstOrDefault());
        }
    }
}
