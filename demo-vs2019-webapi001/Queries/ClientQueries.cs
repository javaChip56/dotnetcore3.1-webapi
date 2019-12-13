using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using demo_vs2019_webapi001.Models;
using demo_vs2019_webapi001.Queries.Interfaces;

namespace demo_vs2019_webapi001.Queries
{
    public class ClientQueries : IClientQueries
    {
        private ClientDBContext _context = null;
        public ClientQueries(ClientDBContext context)
        {
            this._context = context;
        }
        public async Task<Client> GetClientAsync(int id)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Client> GetClientAsync(string clientNo)
        {
            return await _context.Clients.FirstOrDefaultAsync(c =>
                String.Equals(c.Clientno, clientNo, StringComparison.InvariantCultureIgnoreCase));
        }
        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _context.Clients.ToListAsync();
        }
        public async Task<bool> InsertClientAsync(Client client)
        {
            _context.Clients.Add(client);
            int result = await _context.SaveChangesAsync();

            return result >= 0;
        }
        public async Task<bool> UpdateClientAsync(Client client)
        {
            int result = 0;
            var cl = await _context.Clients.FirstOrDefaultAsync(c => c.Id == client.Id);
            if (cl != null)
            {
                cl.Clientno = client.Clientno;
                cl.Clientname = client.Clientname;
                cl.Birthdate = client.Birthdate;
                cl.Updatedby = client.Updatedby;
                cl.Updateddate = client.Updateddate;
                _context.Update(cl);
                result = await _context.SaveChangesAsync();
                return result >= 0;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> DeleteClientAsync(int Id)
        {
            int result = 0;
            var cl = await _context.Clients.FirstOrDefaultAsync(c => c.Id == Id);
            if (cl != null)
            {
                _context.Remove(cl);
                result = await _context.SaveChangesAsync();
                return result >= 0;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> DeleteClientByNoAsync(string clientNo)
        {
            int result = 0;
            var cl = await _context.Clients.FirstOrDefaultAsync(c =>
                String.Equals(c.Clientno, clientNo, StringComparison.InvariantCultureIgnoreCase));
            if (cl != null)
            {
                _context.Remove(cl);
                result = await _context.SaveChangesAsync();
                return result >= 0;
            }
            else
            {
                return false;
            }
        }
    }
}
