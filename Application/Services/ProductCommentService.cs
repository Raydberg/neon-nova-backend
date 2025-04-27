using Application.DTOs.ComentDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ProductCommentService : IProductCommentService
    {
        private readonly IProductCommentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProductRepository _productRepository;
        private IProductCommentService _productCommentServiceImplementation;

        public ProductCommentService (
            IProductCommentRepository repository,
            IMapper mapper,
            ICurrentUserService currentUserService,IProductRepository productRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _productRepository = productRepository;
        }


        public async Task<CommentDto> AddCommentAsync(int productId, CreateCommentDto dto)
        {
            var user = await _currentUserService.GetUser();
            if (user == null)
                throw new InvalidOperationException("Usuario no autenticado.");
            
            var userId = user.Id;

            // Verificar si el usuario ya comentó este producto
            if (await _repository.ExistsByProductAndUserAsync(productId, userId))
                throw new InvalidOperationException("Ya has comentado este producto.");

            var comment = new ProductComment
            {
                ProductId = productId,
                UserId = userId,  
                Comment = dto.Comment,
                Rating = dto.Rating,
                Date = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(comment);
            await _productRepository.UpdateProductPunctuationAsync(productId);
            return _mapper.Map<CommentDto>(created);
        }

        public async Task<CommentDto> UpdateCommentAsync(int commentId, UpdateCommentDto dto)
        {
            var user = await _currentUserService.GetUser();
            if (user == null)
                throw new InvalidOperationException("Usuario no autenticado.");

            var userId = user.Id; // Directamente como string

            // Obtener el comentario por el commentId
            var comment = await _repository.GetByIdAsync(commentId);
            if (comment == null)
                throw new KeyNotFoundException("No existe tu comentario para actualizar.");

            // Verifica que el comentario pertenezca al usuario autenticado
            if (comment.UserId != userId)
                throw new KeyNotFoundException("No existe tu comentario para actualizar.");

            // Mapear los cambios del DTO al comentario
            _mapper.Map(dto, comment);
            comment.Date = DateTime.UtcNow;

            // Actualizar el comentario en la base de datos
            var updated = await _repository.UpdateAsync(comment);

            // Mapear el comentario actualizado a DTO y devolverlo
            return _mapper.Map<CommentDto>(updated);
        }
        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _repository.GetByIdAsync(id);
            if (comment == null)
                throw new KeyNotFoundException("No se encontró el comentario con ese Id.");

            var user = await _currentUserService.GetUser();
            if (user == null)
                throw new UnauthorizedAccessException("Usuario no autenticado.");

            // Verificar que el comentario pertenece al usuario autenticado
            if (comment.UserId != user.Id)
                throw new UnauthorizedAccessException("No tienes permisos para eliminar este comentario.");

            // Si pasa todas las validaciones, eliminar el comentario
            if (!await _repository.DeleteAsync(id))
                throw new KeyNotFoundException("No se pudo eliminar el comentario.");
        }

        public async Task<bool> HasUserCommentedAsync(int productId, string userId)
        {
            return await _repository.ExistsByProductAndUserAsync(productId, userId);
        }
        public async Task<List<CommentDto>> GetCommentsByProductIdAsync(int productId)
        {
            // Obtener los comentarios desde el repositorio, en lugar de _context
            var comments = await _repository.GetCommentsByProductIdAsync(productId);

            // Convertir los comentarios a DTOs para devolver solo los datos necesarios
            return _mapper.Map<List<CommentDto>>(comments);
        }

        public async Task<PagedResult<CommentDto>> GetPaginatedCommentsByProductIdAsync(int productId, int pageNumber, int pageSize)
        {
            var pagedComments = await _repository.GetPaginatedCommentsByProductIdAsync(productId, pageNumber, pageSize);
    
            var commentDtos = _mapper.Map<List<CommentDto>>(pagedComments.Items);
    
            return new PagedResult<CommentDto>
            {
                Items = commentDtos,
                PageNumber = pagedComments.PageNumber,
                PageSize = pagedComments.PageSize,
                TotalCount = pagedComments.TotalCount,
                TotalPages = pagedComments.TotalPages
            };
        }
    }
}

