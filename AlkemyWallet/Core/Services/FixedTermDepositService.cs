﻿using AlkemyWallet.Core.Helper;
using AlkemyWallet.Core.Interfaces;
using AlkemyWallet.Core.Models;
using AlkemyWallet.Entities;
using AlkemyWallet.Entities.Paged;
using AlkemyWallet.Repositories.Interfaces;

using AutoMapper;
using System.Linq;
using System.Xml.Linq;


namespace AlkemyWallet.Core.Services;

public class FixedTermDepositService : IFixedTermDepositService
{
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;

    private readonly IUnitOfWork _unitOfWork;

    public FixedTermDepositService(IUnitOfWork unitOfWork, IMapper mapper)
    {

        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

        public async Task<IEnumerable<FixedTermDeposit>> GetFixedTermDeposits()
        {
            var fixedTermDeposits = await _unitOfWork.FixedTermDepositRepository!.GetAll();
            return fixedTermDeposits;
        }
      public async Task<IEnumerable<FixedTermDeposit>> GetFixedTermDepositsByUserId(int userId)
        {
            var fixedTermDepositsByUserId = await _unitOfWork.FixedTermDepositDetailsRepository!.GetByUser(userId);
            return fixedTermDepositsByUserId;
        }


    public async Task<bool> DeleteFixedTermDeposit(int id)
    {
        await _unitOfWork.FixedTermDepositRepository.Delete(id);
        return true;
    }

    public PagedList<FixedTermDeposit> GetFixedPaged(PageResourceParameters pageResourceParameters)
    {
        if (pageResourceParameters == null)
        {
            throw new ArgumentNullException(nameof(pageResourceParameters));
        }

        var collection = _unitOfWork.FixedTermDepositRepository.GetAll().Result.AsQueryable().OrderBy(x => x.Id);

        var col = collection.Where(x => x.User_id.Equals(pageResourceParameters.UserID));

        return PagedList<FixedTermDeposit>.Create(col,
            pageResourceParameters.Page,
            pageResourceParameters.PageSize);
    }
}