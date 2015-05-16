using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using LINQtoCSV;

namespace Agrimanagr.DataImporter.Lib.ImportServices
{
    internal class CsvHandlerService : ICsvHandlerService
    {
        #region Implementation of ICsvHandlerService

        public Task<IEnumerable<MasterImportEntity>> ReadFromCsVFileAsync(string filePath)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(filePath))
                    return new List<MasterImportEntity>();
                try
                {

                    var inputFileDescription = new CsvFileDescription
                    {
                        // cool - I can specify my own separator!
                        SeparatorChar = ',',
                        FirstLineHasColumnNames = false,
                        EnforceCsvColumnAttribute = false
                    };


                    var imports = new CsvContext().Read<MasterImportEntity>(filePath, inputFileDescription);

                    return imports;
                }
                catch (FileNotFoundException ex)
                {
                    throw ex;
                }
                catch (FieldAccessException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Importer Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return null;
                }
            });
        }

        #endregion
    }
}
