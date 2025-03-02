
using AutoMapper;
using Microsoft.AspNetCore.Hosting;

namespace GameZone.Services.Services.GamesServices
{
    public class GamesServices : IGamesServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostingEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly string _imagesPath;

        public GamesServices(IUnitOfWork unitOfWork,
            IHostingEnvironment webHostEnvironment,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _imagesPath = $"{_webHostEnvironment.WebRootPath}{FileSettings.ImagesPath}/games";
        }

        public async Task Create(CreateGameVM model)
        {
            var game = _mapper.Map<Game>(model);

            game.Cover = await SaveCover(model.Cover);

            _unitOfWork.Games.Create(game);
            await _unitOfWork.Complete();
        }

        public async Task<IEnumerable<Game?>> GetAll()
        {
            var games = await _unitOfWork.Games
                .GetAll(includes: new[] { "category", "Devices" });
            return games
                .OrderBy(g => g.Name)
                .ToList();
        }

        public async Task<Game?> GetById(int id)
            => await _unitOfWork.Games
            .Find(g => g.Id == id, includes: new[] { "category", "Devices" });

        public async Task<Game?> Update(EditGameVM model)
        {
            var game = await _unitOfWork.Games
                .FindWithTrack(g => g.Id == model.Id,
                includes: new[] { "Devices", "GameDevices" });

            if (game is null)
                return null;

            var hasNewCover = model.Cover is not null;
            var oldCover = game.Cover;

            _mapper.Map(model, game);

            if (hasNewCover)
            {
                game.Cover = await SaveCover(model.Cover!);
            }

            _unitOfWork.Games.Update(game);
            var affectedRows = await _unitOfWork.Complete();

            if (affectedRows > 0)
            {
                if (hasNewCover && !string.IsNullOrEmpty(oldCover))
                {
                    var coverPath = Path.Combine(_imagesPath, oldCover);
                    File.Delete(coverPath);
                }

                return game;
            }
            else
            {
                if (hasNewCover && !string.IsNullOrEmpty(game.Cover))
                {
                    var coverPath = Path.Combine(_imagesPath, game.Cover);
                    File.Delete(coverPath);
                }

                return null;
            }
        }

        public async Task<Game?> Delete(int id)
        {
            var game = await _unitOfWork.Games
                .FindWithTrack(g => g.Id == id);

            if (game is null)
                return null!;

            _unitOfWork.Games.Delete(game);
            var effectedRows = await _unitOfWork.Complete();

            if (effectedRows > 0)
            {
                var cover = Path.Combine(_imagesPath, game.Cover);
                File.Delete(cover);
            }
            return game;
        }

        private async Task<string> SaveCover(IFormFile cover)
        {
            var coverName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}";

            var path = Path.Combine(_imagesPath, coverName);

            using var stream = File.Create(path);
            await cover.CopyToAsync(stream);

            return coverName;
        }
    }
}
