﻿using Campervibe.Data.Core;
using System;

namespace Campervibe.Data.Common
{
    public class GenericContextProvider : IContextProvider
    {
        private Context _context;

        public Context GetContext()
        {
            if(_context == null)
            {
                _context = new Context();
            }

            return _context;
        }

        public void SaveChanges()
        {
            GetContext().SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GenericContextProvider() 
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) 
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
    }
}
