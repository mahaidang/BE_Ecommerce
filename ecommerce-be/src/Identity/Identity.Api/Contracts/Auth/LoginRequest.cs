﻿namespace Identity.Api.Contracts.Auth;

public sealed record LoginRequest(string UsernameOrEmail, string Password);
