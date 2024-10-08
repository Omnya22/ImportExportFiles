﻿namespace ImportExportFiles.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    Task<int> CompleteAsync();
    void ChangeTracker(bool enabled);
}
