using AutoMapper;
using CouponAPI;
using CouponAPI.Data;
using CouponAPI.Models;
using CouponAPI.Models.DTO;
using CouponAPI.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddScoped<IValidator<CouponCreateDTO>, CouponCreateValidation>();
builder.Services.AddScoped<IValidator<CouponUpdateDTO>, CouponUpdateValidation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/coupon", () => {
    APIResponse response = new();
    response.Result = CouponStore.couponList;
    response.IsSuccesful = true;
    response.StatusCode = System.Net.HttpStatusCode.OK;
    return Results.Ok(response);
})
    .WithName("GetCoupons")
    .Produces<APIResponse>(200);

app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    APIResponse response = new();
    response.Result = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
    response.IsSuccesful = true;
    response.StatusCode = System.Net.HttpStatusCode.OK;
    return Results.Ok(response);
})
    .WithName("GetCoupon")
    .Produces<APIResponse>(200)
    .Produces(404);

app.MapPost("/api/coupon", async (IMapper _mapper, IValidator <CouponCreateDTO> _validation, [FromBody] CouponCreateDTO coupon_C_DTO) =>
{
    APIResponse response = new();
    var validationResult = await _validation.ValidateAsync(coupon_C_DTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages = validationResult.Errors.Select(e => e.ToString()).ToList();
        response.StatusCode = System.Net.HttpStatusCode.BadRequest;
        return Results.BadRequest(response);
    }
    else if (CouponStore.couponList.FirstOrDefault(c => c.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
    {
        response.ErrorMessages.Add("Coupon Name already exists");
        response.StatusCode = System.Net.HttpStatusCode.BadRequest;
        return Results.BadRequest(response);
    }
    else
    {
        Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);
        coupon.Id = CouponStore.couponList.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
        coupon.Created = DateTime.Now;
        CouponStore.couponList.Add(coupon);
        CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);
        response.Result = couponDTO;
        response.IsSuccesful = true;
        response.StatusCode = System.Net.HttpStatusCode.OK;
        return Results.CreatedAtRoute($"GetCoupon", new { id = couponDTO.Id }, response);
    }
})
    .WithName("CreateCoupon")
    .Accepts<CouponCreateDTO>("application/json")
    .Produces<APIResponse>(201)
    .Produces<APIResponse>(400);

app.MapPut("/api/coupon", async (IMapper _mapper, IValidator<CouponUpdateDTO> _validation, [FromBody] CouponUpdateDTO coupon_U_DTO) =>
{
    APIResponse response = new();
    var validationResult = await _validation.ValidateAsync(coupon_U_DTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages = validationResult.Errors.Select(e => e.ToString()).ToList();
        response.StatusCode = System.Net.HttpStatusCode.BadRequest;
        return Results.BadRequest(response);
    }
    else if (CouponStore.couponList.FirstOrDefault(c => c.Name.ToLower() == coupon_U_DTO.Name.ToLower()) == null)
    {
        response.ErrorMessages.Add("Coupon Name doesn't exists");
        response.StatusCode = System.Net.HttpStatusCode.NotFound;
        return Results.NotFound(response);
    }
    else
    {
        Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(u => u.Id == coupon_U_DTO.Id);
        couponFromStore.Name = coupon_U_DTO.Name;
        couponFromStore.IsActive = coupon_U_DTO.IsActive;
        couponFromStore.Percent = coupon_U_DTO.Percent;
        couponFromStore.LastUpdated = DateTime.Now;

        response.Result = couponFromStore;
        response.IsSuccesful = true;
        response.StatusCode = System.Net.HttpStatusCode.OK;
        return Results.Ok(response);
    }
})
    .WithName("UpdateCoupon")
    .Accepts<CouponUpdateDTO>("application/json")
    .Produces<APIResponse>(200)
    .Produces<APIResponse>(400);

app.MapDelete("/api/coupon/{id:int}", (int id) =>
{

    APIResponse response = new();
    if (CouponStore.couponList.FirstOrDefault(c => c.Id == id) == null)
    {
        response.ErrorMessages.Add("Coupon Id doesn't exists");
        response.StatusCode = System.Net.HttpStatusCode.NotFound;
        return Results.NotFound(response);
    }
    else
    {
        Coupon coupon = CouponStore.couponList.FirstOrDefault(u => u.Id == id);
        CouponStore.couponList.Remove(coupon);
        response.Result = "Deleted successfully";
        response.IsSuccesful = true;
        response.StatusCode = System.Net.HttpStatusCode.OK;
        return Results.Ok(response);
    }
})
    .WithName("Delete Coupon")
    .Produces<APIResponse>(200)
    .Produces<APIResponse>(404)
    .Produces(404);

app.Run();
