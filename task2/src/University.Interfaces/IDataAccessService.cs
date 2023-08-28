using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using University.Interfaces;

namespace University.Services
{
    public interface IDataAccessService
    {
        void SaveDataToFile(string filePath);
        void ReadDataFromFile(string filePath);
        void AddEntity(IEntity entity);
        void RemoveEntity(IEntity entity);
        void EditEntity(IEntity entity);
        ObservableCollection<T> GetEntities<T>() where T : IEntity;
        T FindEntity<T>(string key) where T : IEntity;
        void EnsureDeleted();
        void SetDatabaseContext(DbContext context);
    }
}