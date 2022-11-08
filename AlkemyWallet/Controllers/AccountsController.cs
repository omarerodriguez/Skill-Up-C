﻿using AlkemyWallet.Core.Interfaces;
using AlkemyWallet.Core.Models;
using AlkemyWallet.Core.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlkemyWallet.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountsService;
    private readonly IMapper _mapper;

    public AccountsController(IAccountService accountsService, IMapper mapper)
    {
        _accountsService = accountsService;
        _mapper = mapper;
    }

    //Get all accounts
    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        var accounts = await _accountsService.GetAccounts();

        return Ok(accounts);
    }

    //Get account by id
    [Authorize(Roles = "Administrador")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccountById(int id)
    {
        var account = await _accountsService.GetAccountById(id);

        if (account is null) return NotFound("No existe ninguna cuenta con el id especificado");

        return Ok(account);
    }    

    [Authorize(Roles = "Standard")]
    [HttpPost]
    public async Task<ActionResult> PostAccount([FromForm] AccountForCreationDTO accountDTO)
    {
        await _accountsService.InsertAccounts(accountDTO);
        return Ok("Se ha creado la cuenta exitosamente");
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}")]
    public async Task<ActionResult> PutCatalogue(int id, [FromForm] AccountForUpdateDTO accountDTO)
    {
        var result = await _accountsService.UpdateAccount(id, accountDTO);
        if (!result) return NotFound("Cuenta No Encontrado");
        return Ok("Cuenta Modificada con exito");
    }

    [Authorize(Roles = "Standard")]
    [HttpPost("{id}/deposit")]
    public async Task<ActionResult> PostDeposit(int id, int amount)
    {
        var userIdFromToken = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("uid"))!.Value;
        if (Int32.Parse(userIdFromToken) != id)
            return BadRequest("El id de cuenta ingresado no coincide con el id de usuario registrado en el sistema");

        var result = await _accountsService.Deposit(id, amount);
        if (result.Success)
            return Ok(result.Message);

        return BadRequest(result.Message);


    }

   [Authorize(Roles = "Standard")]
    [HttpPost("{id}/transfer")]
    public async Task<ActionResult> PostTransfer(int id, int amount, int toAccountId)
    {
        var userIdFromToken = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("uid"))!.Value;
        if (Int32.Parse(userIdFromToken) != id)
            return BadRequest("El id de cuenta ingresado no coincide con el id de usuario registrado en el sistema");

        var result = await _accountsService.Transfer(id, amount, toAccountId);
        if (result.Success)
            return Ok(result.Message);
        return BadRequest(result.Message);


    }
}