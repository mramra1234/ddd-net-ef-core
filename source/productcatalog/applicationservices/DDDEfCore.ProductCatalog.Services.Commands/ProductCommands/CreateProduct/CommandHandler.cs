﻿using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.CreateProduct
{
    public class CommandHandler : AsyncRequestHandler<CreateProductCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Product> _repository;
        private readonly AbstractValidator<CreateProductCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory, AbstractValidator<CreateProductCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Product>();
            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #region Overrides of AsyncRequestHandler<CreateProductCommand>

        protected override async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);

            var product = Product.Create(request.ProductName);

            await this._repository.AddAsync(product);
        }

        #endregion
    }
}
//TODO: Missing UnitTest