﻿using AlkemyWallet.Entities;

namespace AlkemyWallet.Repositories.Interfaces;

public interface IRepositoryBase<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<T> GetById(int id);
    Task Insert(T entity);
    Task Update(T entity);
    Task Delete(int id);
}