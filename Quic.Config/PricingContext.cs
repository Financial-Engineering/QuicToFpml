using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Quic.Config
{
    /// <summary>
    /// Set of files necessary for running quicscript batches
    /// </summary>
    public class PricingContext
    {
        /// <summary>
        /// Property attribute to control whether the file associated with property is copied
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        private class DoNotCopy : Attribute
        {
        }

        #region Private Fields
        private string _aggregation;
        private string _cptyFile;
        private string _csvResults;
        private string _exportParams;
        private string _h5Results;
        private string _holidaysH5;
        private string _legalDoc;
        private string _marketDataH5;
        private string _moduleBindings;
        private string _observables;
        private string _paramSet;
        private string _portfolio;
        private string _riskFactorParamH5;
        private string _riskMeasurePlugin;
        private string _schedules;
        private string _simParams;
        private string _siteInfo;
        private string _structureNodePlugin;
        private string _mtmPlugin;
        private string _mtmH5Results;
        private string _mtmCsvResults;
        private string _mtmExportParams;
        private string _marketDataCsv;
        private string _holidaysCsv;
        private string _pricingDB;

        #endregion 

        [DoNotCopy]
        public string Path { get; set; }

        public string SiteInfo
        {
            get { return FullName(_siteInfo); }
            set { _siteInfo = value; }
        }

        public string PricingDB
        {
            get { return _pricingDB; }
            set { _pricingDB = value; }

        }


        public string StructureNodePlugin
        {
            get { return FullName(_structureNodePlugin); }
            set { _structureNodePlugin = value; }
        }

        public string RiskMeasurePlugin
        {
            get { return FullName(_riskMeasurePlugin); }
            set { _riskMeasurePlugin = value; }
        }

        public string RiskFactorParamH5
        {
            get { return FullName(_riskFactorParamH5); }
            set { _riskFactorParamH5 = value; }
        }

        public string ModuleBindings
        {
            get { return FullName(_moduleBindings); }
            set { _moduleBindings = value; }
        }

        public string Observables
        {
            get { return FullName(_observables); }
            set { _observables = value; }
        }

        public string MarketDataH5
        {
            get { return FullName(_marketDataH5); }
            set { _marketDataH5 = value; }
        }

        public string HolidaysH5
        {
            get { return FullName(_holidaysH5); }
            set { _holidaysH5 = value; }
        }        

        [DoNotCopy]
        public string MarketDataCsv
        {
            get { return FullName(_marketDataCsv); }
            set { _marketDataCsv = value; }
        }

        [DoNotCopy]
        public string HolidaysCsv
        {
            get
            {
                return FullName(_holidaysCsv);
            }
            set { _holidaysCsv = value; }
        }

        [DoNotCopy]
        public string Schedules
        {
            get { return FullName(_schedules); }
            set { _schedules = value; }
        }

        [DoNotCopy]
        public string CptyHierarchy
        {
            get { return FullName(_cptyFile); }
            set { _cptyFile = value; }
        }

        [DoNotCopy]
        public string LegalDocument
        {
            get { return FullName(_legalDoc); }
            set { _legalDoc = value; }
        }

        [DoNotCopy]
        public string Portfolio
        {
            get { return FullName(_portfolio); }
            set { _portfolio = value; }
        }

        public string Aggregation
        {
            get { return FullName(_aggregation); }
            set { _aggregation = value; }
        }

        public string ExportParams
        {
            get { return FullName(_exportParams); }
            set { _exportParams = value; }
        }

        public string MtmExportParams
        {
            get { return FullName(_mtmExportParams); }
            set { _mtmExportParams = value; }
        }

        public string SimParams
        {
            get { return FullName(_simParams); }
            set { _simParams = value; }
        }

        public string H5Results
        {
            get { return FullName(_h5Results); }
            set { _h5Results = value; }
        }

        public string CsvResults
        {
            get { return FullName(_csvResults); }
            set { _csvResults = value; }
        }

        public string ModelParamSet
        {
            get { return FullName(_paramSet); }
            set { _paramSet = value; }
        }

        public string MtmPlugin
        {
            get { return FullName(_mtmPlugin); }
            set { _mtmPlugin = value; } 
        }

        public string MtmH5Results
        {
            get { return FullName(_mtmH5Results); }
            set { _mtmH5Results = value; } 
        }

        public string MtmCsvResults
        {
            get { return FullName(_mtmCsvResults); }
            set { _mtmCsvResults = value; } 
        }

        private string FullName(string name)
        {
            return name == null ? "" : System.IO.Path.Combine(Path, name);
        }

        /// <summary>
        /// Makes a copy of the context by copying files not marked with "DoNotCopy" to the destination 
        /// </summary>
        /// <param name="dst">Destination context</param>
        public void Copy(PricingContext dst)
        {
            foreach (var p in GetType().GetProperties()
                .Where(p => p.GetCustomAttribute(typeof (DoNotCopy)) == null)
                .Where(p => File.Exists(p.GetValue(this).ToString())))
            {
                File.Copy(p.GetValue(this).ToString(), p.GetValue(dst).ToString(), true);
            }
        }

        /// <summary>
        /// Delete all files in a context
        /// </summary>
        public void Delete()
        {
            foreach (var p in GetType().GetProperties()
                .Where(p => p.Name != "Path")
                .Where(p => File.Exists(p.GetValue(this).ToString())))
            {
                File.Delete(p.GetValue(this).ToString());
            }
        }

        private static string GetTemporaryDirectory()
        {
            var tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            return tempDir;
        }

        /// <summary>
        /// Will create a new context stored in the Windows temp directory
        /// </summary>
        /// <param name="createTempName">If true, create temporary file names</param>
        /// <returns>New pricing context rooted at %TEMP%</returns>
        public PricingContext CreateTemporary(Boolean createTempName = true)
        {
            var context = MemberwiseClone() as PricingContext;

            if (context != null)
            {
                context.Path = GetTemporaryDirectory();

                if (createTempName)
                {
                    Parallel.ForEach(
                        context.GetType()
                            .GetProperties()
                            .Where(p => p.Name != "Path"), p => p.SetValue(context,
                                Convert.ChangeType(System.IO.Path.GetFileName(System.IO.Path.GetTempFileName()),
                                    p.PropertyType),
                                null));
                }

                Copy(context); 
            }

            return context;
        }

    }
}