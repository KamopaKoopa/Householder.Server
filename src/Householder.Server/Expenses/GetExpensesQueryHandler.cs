using DbReader;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using Householder.Server.Database;

namespace Householder.Server.Expenses
{
    public class GetExpensesQueryHandler : IQueryHandler<GetExpensesQuery, IEnumerable<ExpenseDTO>>
    {
        private IDbConnection dbConnection;
        private ISqlProvider sqlProvider;

        public GetExpensesQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<IEnumerable<ExpenseDTO>> HandleAsync(GetExpensesQuery query, CancellationToken cancellationToken)
        {
            string queryString;
            if (query.PayeeId != null && query.Status != null)
            {
                queryString = sqlProvider.GetExpensesByResidentAndStatus;
            }
            else if (query.PayeeId != null)
            {
                queryString = sqlProvider.GetExpensesByResident;
            }
            else if (query.Status != null)
            {
                queryString = sqlProvider.GetExpensesByStatus;
            }
            else
            {
                queryString = sqlProvider.GetExpenses;
            }

            var results = await dbConnection.ReadAsync<ExpenseDTO>(queryString, query);

            return results;
        }
    }

    public class GetExpensesQuery : IQuery<IEnumerable<ExpenseDTO>>
    {
        public int? Limit { get; set; }
        public int? Status { get; set; }
        public int? PayeeId { get; set; }
    }
}