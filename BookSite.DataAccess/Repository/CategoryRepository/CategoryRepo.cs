﻿using BookSite.DataAccess.Repository.IRepository;
using BookSIte.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.CategoryRepository
{
    public class CategoryRepo : Repository<Category> , ICategoryRepo
    {
        private readonly Context _Context;
        
        public CategoryRepo(Context Context) : base(Context) 
        {
            _Context = Context;
            
        }

        public void Update(Category obj)
        {
            _Context.Update(obj);
        }
    }
}
