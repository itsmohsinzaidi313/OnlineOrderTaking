using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ClientResponseService
{

    [Authorize]
    public class GatewayHub() : Hub
    {
    }
}
