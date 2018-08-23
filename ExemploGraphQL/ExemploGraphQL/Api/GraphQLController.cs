using GraphQL;
using GraphQL.Http;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation.Complexity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ExemploGraphQL
{
    public class GraphQLController : ApiController
    {
        private readonly ISchema _schema;
        private readonly IDocumentExecuter _executer;
        private readonly IDocumentWriter _writer;

        public GraphQLController(IDocumentExecuter executer,
                                 IDocumentWriter writer,
                                 StarsWarsSchema schema)
        {
            _executer = executer;
            _writer = writer;
            _schema = schema;
        }

        [HttpGet]
        public Task<HttpResponseMessage> GetAsync(HttpRequestMessage request)
        {
            try
            {
                return PostAsync(request, new GraphQLQuery { Query = "query{hero{id, name} }", Variables = "" });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync(HttpRequestMessage request, GraphQLQuery query)
        {
            var inputs = query.Variables.ToInputs();
            var queryToExecute = query.Query;

            var result = await this._executer.ExecuteAsync(x =>
            {
                x.Schema = _schema;
                x.Query = queryToExecute;
                x.OperationName = query.OperationName;
                x.Inputs = inputs;

                x.ComplexityConfiguration = new ComplexityConfiguration { MaxDepth = 15 };
                x.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
            }).ConfigureAwait(false);

            var httpResult = result.Errors?.Count > 0
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.OK;

            var json = this._writer.Write(result);

            var response = request.CreateResponse(httpResult);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return response;
        }
    }

    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public string Variables { get; set; }
    }
}

