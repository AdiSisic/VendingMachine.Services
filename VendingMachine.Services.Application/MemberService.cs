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
            BaseResponse<bool> response = new();

            try
            {
                response = await RegisterValidationAsync(user);
                if (string.IsNullOrWhiteSpace(response.Message))
                {
                    // CREATE NEW USER ONLY IF VALIDATION HAS PASSED
                    var newUser = _mapper.Map<AppModels.User, DomainModels.User>(user);
                    newUser.Password = HashString(user.Password);

                    await _userRepository.CreateUserAsync(newUser);

                    response.Data = newUser.Id != 1;
                    response.Success = true;
                }
            }
            catch (Exception)
            {
                // TODO: CREATE LOG
            }

            return response;
        }

        public async Task<BaseResponse<AppModels.User>> LoginAsync(string username, string password)
        {
            BaseResponse<AppModels.User> response = new();

            try
            {
                response = LoginValidation(username, password);
                if (String.IsNullOrWhiteSpace(response.Message))
                {
                    var dbUser = await _userRepository.GetUserAsync(username);
                    if (dbUser == null || dbUser.Password != HashString(password))
                    {
                        response.Message = "Invalid Username or Password";
                    }
                    else
                    {
                        response.Data = _mapper.Map<DomainModels.User, AppModels.User>(dbUser);
                        response.Success = true;
                    }
                }
            }
            catch (Exception)
            {
                //TODO: CREATE LOG
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteUserAsync(int userId)
        {
            BaseResponse<bool> response = new();

            try
            {
                var dbUser = await _userRepository.GetUserAsync(userId);

                if (dbUser == null)
                {
                    response.Message = "Invalid user Id";
                }
                else
                {
                    await _userRepository.DeleteUserAsync(dbUser);

                    response.Data = true;
                    response.Success = true;
                }
            }
            catch (Exception)
            {
                // TODO: CREATE LOG
            }

            return response;
        }

        #endregion << Public Methods >>

        #region << Private Methods >>

        private string HashString(string text)
        {
            return BitConverter.ToString(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(text)))
                               .Replace("-", string.Empty);
        }

        private async Task<BaseResponse<bool>> RegisterValidationAsync(AppModels.User user)
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
                response.Message = "Username has not been provided";
                return response;
            }


            // Check if user exists
            bool usernameTaken = await _userRepository.UserExitsAsync(user.Username);
            if (usernameTaken)
            {
                // TODO: CREATE LOG
                response.Message = "Username is already taken";
                return response;
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
