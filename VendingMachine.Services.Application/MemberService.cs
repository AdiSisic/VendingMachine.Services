using AutoMapper;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VendingMachine.Services.Api.Base;
using VendingMachine.Services.Application.Abstractions;
using VendingMachine.Services.Application.Abstractions.Repositories;

using AppModels = VendingMachine.Services.Application.Models;
using DomainModels = VendingMachine.Services.Domain;

namespace VendingMachine.Services.Application
{
    public class MemberService : IMemberService
    {
        #region << Fields >>

        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        #endregion << Fields >>

        #region << Constructor >>

        public MemberService(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        #endregion << Constructor >>

        #region << Public Methods >>

        public async Task<BaseResponse<bool>> RegisterAsync(AppModels.User user)
        {
            BaseResponse<bool> baseResponse = new();

            try
            {
                baseResponse = await MemberManipulationValidationAsync(user);

                if (string.IsNullOrWhiteSpace(baseResponse.Message))
                {
                    // CREATE NEW USER ONLY IF VALIDATION HAS PASSED
                    var newUser = _mapper.Map<AppModels.User, DomainModels.User>(user);
                    newUser.Password = HashString(user.Password);

                    await _userRepository.CreateUserAsync(newUser);

                    baseResponse.Data = newUser.Id != 1;
                    baseResponse.Success = true;
                }
            }
            catch (Exception)
            {
                baseResponse.Message = "Something went wrong. Please contact support for more details";
            }

            return baseResponse;
        }

        public async Task<BaseResponse<bool>> UpdateUserAsync(AppModels.User user)
        {
            BaseResponse<bool> baseResponse = new();

            try
            {
                baseResponse = await MemberManipulationValidationAsync(user);
                if(string.IsNullOrWhiteSpace(baseResponse.Message))
                {
                    var dbUser = await _userRepository.GetUserAsync(user.Id, false);
                    var mappedUser = _mapper.Map<AppModels.User, DomainModels.User>(user);
                    mappedUser.Deposit = dbUser.Deposit;
                    mappedUser.Password = HashString(user.Password);

                    await _userRepository.UpdateUserAsync(mappedUser);
                    baseResponse.Data = true;
                    baseResponse.Success = true;
                }
            }
            catch (Exception)
            {
                baseResponse.Message = "Something went wrong. Please contact support for more details";
            }

            return baseResponse;
        }

        public async Task<BaseResponse<AppModels.User>> LoginAsync(string username, string password)
        {
            BaseResponse<AppModels.User> baseResponse = new();

            try
            {
                baseResponse = LoginValidation(username, password);
                if (String.IsNullOrWhiteSpace(baseResponse.Message))
                {
                    var dbUser = await _userRepository.GetUserAsync(username);
                    if (dbUser == null || dbUser.Password != HashString(password))
                    {
                        baseResponse.Message = "Invalid Username or Password";
                    }
                    else
                    {
                        baseResponse.Data = _mapper.Map<DomainModels.User, AppModels.User>(dbUser);
                        baseResponse.Success = true;
                    }
                }
            }
            catch (Exception)
            {
                baseResponse.Message = "Something went wrong. Please contact support for more details";
            }

            return baseResponse;
        }

        public async Task<BaseResponse<bool>> DeleteUserAsync(int userId)
        {
            BaseResponse<bool> baseResponse = new();

            try
            {
                var dbUser = await _userRepository.GetUserAsync(userId);

                if (dbUser == null)
                {
                    baseResponse.Message = "Invalid user Id";
                    return baseResponse;
                }
                await _userRepository.DeleteUserAsync(dbUser);

                baseResponse.Data = true;
                baseResponse.Success = true;
            }
            catch (Exception)
            {
                baseResponse.Message = "Something went wrong. Please contact support for more details";
            }

            return baseResponse;
        }

        #endregion << Public Methods >>

        #region << Private Methods >>

        private string HashString(string text)
        {
            return BitConverter.ToString(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(text)))
                               .Replace("-", string.Empty);
        }

        private async Task<BaseResponse<bool>> MemberManipulationValidationAsync(AppModels.User user, bool skipDbUserCheck = false)
        {
            BaseResponse<bool> response = new();

            if (user == null)
            {
                response.Message = "User has not been provided";
                return response;
            }

            if (string.IsNullOrWhiteSpace(user?.Username))
            {
                response.Message = "Username has not been provided";
                return response;
            }

            if (string.IsNullOrWhiteSpace(user?.Password))
            {
                response.Message = "Password has not been provided";
                return response;
            }

            // Check if different user exists
            var dbUser = await _userRepository.GetUserAsync(user.Username);
            if (dbUser != null)
            {
                if (user.Id != dbUser.Id)
                {
                    response.Message = "Username is already taken";
                    return response;
                }

                user.Deposit = dbUser.Deposit;
            }

            return response;
        }

        private BaseResponse<AppModels.User> LoginValidation(string username, string password)
        {
            BaseResponse<AppModels.User> response = new();

            if (string.IsNullOrWhiteSpace(username))
            {
                response.Message = "Username has not been provided";
                return response;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                response.Message = "Username has not been provided";
                return response;
            }

            return response;
        }

        #endregion << Private Methods >>
    }
}
