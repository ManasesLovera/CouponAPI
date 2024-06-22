# CouponAPI

## Practicing about Minimal APIs with ASP .NET Core

Youtube video: https://www.youtube.com/watch?v=lFo3Yy8Ro7w&t=1268s&ab_channel=freeCodeCamp.org

### Dependencies or libraries:
- AutoMapper
- FluentValidation

### Issues due to the version

In the youtube video he is using .NET 7 and I used .NET 8. These are the following changes I found due to the version:
1. Don't install `AutoMapper.Extensions.Microsoft.DependencyInjection`, just install AutoMapper or you will get an error saying: `The call is ambiguous between the following methods or porperties` this happens because the function you will use `.AddAutoMapper` it's already in the `AutoMapper` dependency and you don't need to add another library for that, AutoMapper already includes it.
2. Don't use `builder.Services.AddValidatorFromAssemblyContaining` when working with FluentValidation library for solving the exception, it won't work use `builder.Services.AddScoped<IValidator<CouponCreateDTO>, CouponCreateValidation>();` instead, please review the FluentValidation docs in the Minimal APIs section [here](https://docs.fluentvalidation.net/en/latest/aspnet.html#minimal-apis).

## Docs

This is an API for Coupons, it's basically a CRUD, you can create coupons, get them all, get one by Id, update and delete.

The endpoints are:

1. GET -> "/api/coupon": 
	- Return all the coupons in the List in Memory.
```json
{
  "isSuccesful": true,
  "result": [],
  "statusCode": 100,
  "errorMessages": [
    "string"
  ]
}
```
2. GET -> "api/coupon/:id": 
	- Return one coupon by it's Id.
```json
{
  "isSuccesful": true,
  "result": "string",
  "statusCode": 100,
  "errorMessages": [
    "string"
  ]
}
```
3. POST -> "/api/coupon": 
	- Creates a new coupon.
```json
{
    "name": string,
    "percent": string
    "isActive": boolean
}
```
4. PUT -> "/api/coupon":
    - Updates the coupon.
```json
{
    "id": integer
    "name": string
    "percent": number
    "isActive": boolean
}
```

5. DELETE -> "api/coupon/:id"
    - Deletes the coupon using the id in the route.