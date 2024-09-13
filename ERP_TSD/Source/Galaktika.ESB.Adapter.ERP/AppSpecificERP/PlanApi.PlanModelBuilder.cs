using Microsoft.OData.Edm;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Galaktika.ESB.ServiceAdapterApi.ModelBuilder;

namespace Galaktika.ESB.Adapter.ERP
{
    public partial class PlanApi
    {
        private partial class PlanModelBuilder : EsbModelBuilder
		{
			private IEdmModel _model;
			private ApiBase _api;

            public PlanModelBuilder(IServiceProvider serviceProvider) : base(serviceProvider)
            {
            }

            public async override Task<IEdmModel> GetModelAsync(ModelContext context, CancellationToken cancellationToken)
			{
				_api = context.GetApiService<ApiBase>();

				if (_model != null)
					return _model;

				var model = (EdmModel)await base.GetModelAsync(context, cancellationToken);
				BuildModelOperations(model, _api.GetType());

				_model = model;
				return model;

			}

			public override List<ClassToGenerate> FillInQueue()
			{
				var apiType = _api.GetType();
				var props = apiType.GetProperties().Where(p => p.PropertyType.Name == typeof(IQueryable<>).Name).ToList();
				var queue = new List<ClassToGenerate>();
				try
				{
					foreach (var prop in props)
					{
						var type = prop.PropertyType.GenericTypeArguments[0];
						var classToGenerate = new ClassToGenerate();
						var baseType = type.BaseType;
						if (baseType != null)
						{
							classToGenerate.BaseClass = baseType;
							classToGenerate.BaseClassName = baseType.FullName;
						}

						var fullName = type.FullName;
						var nameSpace = type.Namespace ?? "";

						var shortNameEntitySet = type.Name;

						if (type.IsGenericType)
						{
							var simpleName = type.Name.Substring(0, type.Name.Length - 2);
							shortNameEntitySet = string.Format(simpleName + "_" + type.GenericTypeArguments[0].FullName + "_");
						}
						classToGenerate.IsPersistent = true;
						classToGenerate.FullName = fullName;
						classToGenerate.Namespase = nameSpace;
						classToGenerate.OwnClass = type;
						//classToGenerate.ShortNameEntitySet = shortNameEntitySet;
						classToGenerate.ShortNameType = shortNameEntitySet;
						if (type.GetProperties().Any(p => p.CustomAttributes.Any(t => t.AttributeType == typeof(KeyAttribute))))
							classToGenerate.HasKey = true;
						if (!queue.Contains(classToGenerate))
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

			public override void GenerateKeys(EdmEntityType entityType, ClassToGenerate businessObject, IEdmModel model)
			{
				var type = businessObject.OwnClass;
				var keyProp = type.GetProperties()
					.Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute)));
				foreach (var propertyInfo in keyProp)
				{
					var keyProperty = entityType.FindProperty(propertyInfo.Name);
					entityType.AddKeys((IEdmStructuralProperty)keyProperty);
				}
			}
		}
	}
}