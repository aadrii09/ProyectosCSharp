﻿using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repositorio.IRepositorio;

namespace ApiPeliculas.Repositorio
{
    public class CategotiaRepositorio : ICategoriaRepositorio
    {
        private readonly ApplicationDbContext _bd;

        //constructor
        public CategotiaRepositorio(ApplicationDbContext bd)
        {
            _bd = bd;
        }

        public bool ActualizarCategoria(Categoria categoria)
        {
           categoria.FechaCreacion = DateTime.Now;

            //arreglo del problema put

            var categoriaExistente = _bd.Categoria.Find(categoria.Id);
            if (categoriaExistente != null)
            {
                _bd.Entry(categoriaExistente).CurrentValues.SetValues(categoria);
            }
            else
            {
                _bd.Categoria.Update(categoria);
            }
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            _bd.Categoria.Add(categoria);
            return Guardar();
        }

        public bool EliminarCategoria(Categoria categoria)
        {
            _bd.Categoria.Remove(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(int Id)
        {
            return _bd.Categoria.Any(c => c.Id == Id);
        }

        public bool ExisteCategoria(string nombre)
        {
            bool valor = _bd.Categoria.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }

        public Categoria GetCategoria(int CategoriaId)
        {
            return _bd.Categoria.FirstOrDefault(c => c.Id == CategoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _bd.Categoria.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0 ? true : false;
        }   
    }
}
