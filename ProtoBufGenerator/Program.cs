// See https://aka.ms/new-console-template for more information
using PointofSaleModels.Application;
using ProtoGenerationLib;

var generator = new ProtoGenerator();
var protoDefs = generator.GenerateProtos([typeof(CustomerOrder)]);
protoDefs.WriteToFiles(@"D:\source\repos\ziauddin784\OnlineOrderTaking\PointofSaleModels\Protos\");
