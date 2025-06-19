using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SADVO.Application.Dtos;
using SADVO.Application.Interfaces;
using SADVO.Application.ViewModels;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Entities.Dirigente;
using SADVO.Domain.Entities.Elector;
using SADVO.Domain.Interfaces;
using Tesseract;

namespace SADVO.Application.Services
{
    public class VotoService : IVotoService
    {
        private readonly IGenericRepository<Ciudadano> _ciudadanoRepo;
        private readonly IGenericRepository<Eleccion> _eleccionRepo;
        private readonly IGenericRepository<Voto> _votoRepo;
        private readonly IGenericRepository<PuestoElectivo> _puestoRepo;
        private readonly IGenericRepository<CandidatoPuesto> _candidatoPuestoRepo;
        private readonly IMapper _mapper;

        public VotoService(
            IGenericRepository<Ciudadano> ciudadanoRepo,
            IGenericRepository<Eleccion> eleccionRepo,
            IGenericRepository<Voto> votoRepo,
            IGenericRepository<PuestoElectivo> puestoRepo,
            IGenericRepository<CandidatoPuesto> candidatoPuestoRepo,
            IMapper mapper)
        {
            _ciudadanoRepo = ciudadanoRepo;
            _eleccionRepo = eleccionRepo;
            _votoRepo = votoRepo;
            _puestoRepo = puestoRepo;
            _candidatoPuestoRepo = candidatoPuestoRepo;
            _mapper = mapper;
        }

        public async Task<string> ValidarCedulaInicialAsync(string cedula)
        {
            var ciudadano = (await _ciudadanoRepo.FindAsync(c => c.Cedula == cedula)).FirstOrDefault();
            if (ciudadano == null) return "La cédula no está registrada.";
            if (!ciudadano.Activo) return "El ciudadano está inactivo.";

            var eleccionActiva = (await _eleccionRepo.FindAsync(e => e.Activa && !e.Finalizada)).FirstOrDefault();
            if (eleccionActiva == null) return "No hay ningún proceso electoral en estos momentos.";

            var yaVoto = await _votoRepo.ExistsAsync(v => v.CiudadanoId == ciudadano.Id && v.EleccionId == eleccionActiva.Id);
            if (yaVoto) return "Ya ha ejercido su derecho al voto.";

            return "OK";
        }

        public async Task<bool> YaHaVotadoAsync(int ciudadanoId, int eleccionId)
        {
            return await _votoRepo.ExistsAsync(v => v.CiudadanoId == ciudadanoId && v.EleccionId == eleccionId);
        }

        public async Task<string> ProcesarOCRAsync(IFormFile imagen)
        {
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
            string trainedDataPath = Path.Combine(dataPath, "spa.traineddata");

            if (!Directory.Exists(dataPath))
                throw new DirectoryNotFoundException($"La carpeta 'tessdata' no existe en: {dataPath}");

            if (!System.IO.File.Exists(trainedDataPath))
                throw new FileNotFoundException($"El archivo 'spa.traineddata' no fue encontrado en: {trainedDataPath}");

            try
            {
                using var engine = new TesseractEngine(dataPath, "spa", EngineMode.Default);
                using var stream = imagen.OpenReadStream();
                using var img = Pix.LoadFromMemory(ToByteArray(stream));
                using var page = engine.Process(img);

                var text = page.GetText()?.Trim();
                return text ?? "";
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al inicializar Tesseract: {ex.Message}", ex);
            }
        }


        private byte[] ToByteArray(Stream stream)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        public async Task<List<SeleccionCandidatoViewModel>> ObtenerPuestosYOpcionesAsync(int ciudadanoId)
        {
            var eleccion = (await _eleccionRepo.FindAsync(e => e.Activa && !e.Finalizada)).FirstOrDefault();
            if (eleccion == null) return new();

            var puestos = await _puestoRepo.FindAsync(p => p.Activo);

            var candidatosPuestos = await _candidatoPuestoRepo.FindAsync(
                cp => cp.EleccionId == eleccion.Id && cp.Candidato.Activo && cp.PuestoElectivo.Activo,
                include: query => query
                    .Include(cp => cp.Candidato).ThenInclude(c => c.PartidoPolitico)
                    .Include(cp => cp.PuestoElectivo)
            );

            var resultado = new List<SeleccionCandidatoViewModel>();

            foreach (var puesto in puestos)
            {
                var candidatosPuesto = candidatosPuestos
                    .Where(cp => cp.PuestoElectivoId == puesto.Id)
                    .Select(cp => new CandidatoOpcionViewModel
                    {
                        CandidatoId = cp.CandidatoId,
                        NombreCompleto = $"{cp.Candidato.Nombre} {cp.Candidato.Apellido}",
                        Partido = cp.Candidato.PartidoPolitico?.Nombre ?? "Sin partido",
                        Foto = cp.Candidato.FotoUrl,
                        Seleccionado = false
                    }).ToList();

                candidatosPuesto.Add(new CandidatoOpcionViewModel
                {
                    CandidatoId = 0,
                    NombreCompleto = "Ninguno",
                    Partido = "Independiente",
                    Foto = null,
                    Seleccionado = false
                });

                resultado.Add(new SeleccionCandidatoViewModel
                {
                    PuestoElectivoId = puesto.Id,
                    NombrePuesto = puesto.Nombre,
                    Candidatos = candidatosPuesto
                });
            }

            return resultado;
        }

        public async Task<string> RegistrarVotoAsync(List<VotoDto> votosDto, int ciudadanoId)
        {
            var eleccion = (await _eleccionRepo.FindAsync(e => e.Activa && !e.Finalizada)).FirstOrDefault();
            if (eleccion == null) return "No hay elección activa.";

            foreach (var votoDto in votosDto)
            {
                var voto = new Voto
                {
                    CiudadanoId = ciudadanoId,
                    CandidatoId = votoDto.CandidatoId,
                    EleccionId = eleccion.Id,
                    PuestoElectivoId = votoDto.PuestoElectivoId,
                    FechaVoto = DateTime.Now
                };

                await _votoRepo.AddAsync(voto);
            }

            return "OK";
        }
    }
}
