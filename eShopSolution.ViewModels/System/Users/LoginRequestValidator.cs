using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
  public  class LoginRequestValidator:AbstractValidator<LoginRequest>
    {


        public LoginRequestValidator() {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("User name is requird");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password name is requird")
                .MinimumLength(6).WithMessage("Password is at least 6 characters");


                }
    }
}
