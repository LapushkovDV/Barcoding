using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.Atlantis.Api;
using Galaktika.ESB.ServiceAdapterApi.ModelBuilder;
using Microsoft.OData.Edm;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Galaktika.ERP.Adapter.Atlantis
{
    /// <summary>
    /// AtlServices
    /// </summary>
	public partial class AtlServices
	{
		private static readonly ConcurrentDictionary<string, Type> RegisteredTypes = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// AtlantisModelBuilder
        /// </summary>
		public partial class AtlantisModelBuilder : EsbModelBuilder
		{
			private Type _targetType;

			private Type TargetType
			{
				get
				{
					if (_targetType == null)
					{
						var fields = _modelBuilder.GetType().GetFields(
							BindingFlags.NonPublic |
							BindingFlags.Instance);
						_targetType = (Type)fields.Single(f => f.Name == "targetType").GetValue(_modelBuilder);
					}
					return _targetType;
				}
			}

			private readonly IModelBuilder _modelBuilder;

            /// <summary>
            /// AtlantisModelBuilder
            /// </summary>
            /// <param name="serviceProvider">service provider</param>
            /// <param name="modelBuilder">model builder</param>
            public AtlantisModelBuilder(IServiceProvider serviceProvider, IModelBuilder modelBuilder) : base(serviceProvider)
			{
				_modelBuilder = modelBuilder;
			}

			private DbContext _currentContext;
		    private DictionaryApi _dictionaryApi;
			private IEdmModel _model;

            /// <summary>
            /// Get's EDM model asynchronously
            /// </summary>
            /// <param name="context">Model context</param>
            /// <param name="cancellationToken">cancellationToken</param>
            /// <returns>Entity data model</returns>
			public override async Task<IEdmModel> GetModelAsync(ModelContext context, CancellationToken cancellationToken)
			{
				_currentContext = context.GetApiService<DbContext>();
			    _dictionaryApi = context.GetApiService<DictionaryApi>();
				if (_model != null)
					return _model;
				
				var model = (EdmModel)await base.GetModelAsync(context, cancellationToken);
				BuildModelOperations(model, TargetType);

                _model = model;
				return model;
			}

            /// <summary>
            /// Method creates list of entities, that should be added into edm model
            /// </summary>
            /// <returns>List of entities</returns>
            public override List<ClassToGenerate> FillInQueue()
			{
				var queue = new List<ClassToGenerate>();
				try
				{
					foreach (var bo in _currentContext.Model.GetEntityTypes())
					{
						var classToGenerate = new ClassToGenerate();
						var baseType = bo.BaseType;
						if (baseType != null)
						{
							classToGenerate.BaseClass = baseType.ClrType;
							classToGenerate.BaseClassName = baseType.ClrType.FullName;
						}
						var t = bo.ClrType;

					    var fullName = t.FullName;
						var nameSpace = t.Namespace ?? "";

						var shortNameType = string.IsNullOrEmpty(nameSpace) ? bo.Name : bo.Name.Substring(nameSpace.Length + 1);
						var shortNameEntitySet = t.Name;

						if (t.IsGenericType)
						{
							var simpleName = t.Name.Substring(0, t.Name.Length - 2);
							shortNameEntitySet = string.Format(simpleName + "_" + t.GenericTypeArguments[0].FullName + "_");
						}
						classToGenerate.IsPersistent = true;
						classToGenerate.FullName = fullName;
						classToGenerate.Namespase = nameSpace;
						classToGenerate.OwnClass = t;
						classToGenerate.ShortNameEntitySet = shortNameEntitySet;
						classToGenerate.ShortNameType = shortNameType;
						if (bo.GetKeys().Any())
							classToGenerate.HasKey = true;
						if(!queue.Contains(classToGenerate))
                            queue.Add(classToGenerate);
					}
				}
				catch (Exception e)
				{
                    _Warn_Exception_0(e.ToString());
                    throw;
				}
				return queue;
			}

            /// <summary>
            /// Generating keys for entity type
            /// </summary>
            /// <param name="entityType">Entity type instance</param>
            /// <param name="businessObject">Business object</param>
            /// <param name="model">Entity data model</param>
			public override void GenerateKeys(EdmEntityType entityType, ClassToGenerate businessObject, IEdmModel model)
			{
				var entity = _currentContext.Model.FindEntityType(businessObject.FullName);
				var key = entity.GetKeys().Single();
				var keyName = key.Properties[0].Name;
				var keyProperty = entityType.FindProperty(keyName);
				entityType.AddKeys((IEdmStructuralProperty)keyProperty);
			}

		}

        /// <summary>
        /// AtlantisModelMapper
        /// </summary>
		public class AtlantisModelMapper : IModelMapper
		{
			readonly IModelMapper _innerMapper;

            /// <summary>
            /// AtlantisModelMapper
            /// </summary>
            /// <param name="innerMapper">inner mapper</param>
			public AtlantisModelMapper(IModelMapper innerMapper)
			{
				_innerMapper = innerMapper;
			}

            /// <summary>
            /// Try get relevant type from registered types
            /// </summary>
            /// <param name="context">ModelContext</param>
            /// <param name="name">Type name</param>
            /// <param name="relevantType">Relevant type</param>
            /// <returns>true if succeded, false if not</returns>
			public bool TryGetRelevantType(ModelContext context, string name, out Type relevantType)
			{
				if (RegisteredTypes.TryGetValue(name, out relevantType))
					return true;

				var result = _innerMapper.TryGetRelevantType(context, name, out relevantType);
				return result;
			}

            /// <summary>
            /// Try get relevant type from registered types
            /// </summary>
            /// <param name="context">ModelContext</param>
            /// <param name="namespaceName">Namespace of the type</param>
            /// <param name="name">Type name</param>
            /// <param name="relevantType">Relevant type</param>
            /// <returns>true if succeded, false if not</returns>
            public bool TryGetRelevantType(ModelContext context, string namespaceName, string name, out Type relevantType)
			{
				var fullname = string.IsNullOrEmpty(namespaceName) ? name : namespaceName + "." + name;
				if (RegisteredTypes.TryGetValue(fullname, out relevantType))
					return true;

				var result = _innerMapper.TryGetRelevantType(context, namespaceName, name, out relevantType);
				return result;
			}
		}
	}


}
