using System;
using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.container.impl.jboss.util
{
	using AbstractAttributeDefinitionBuilder = org.jboss.@as.controller.AbstractAttributeDefinitionBuilder;
	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;
	using MapAttributeDefinition = org.jboss.@as.controller.MapAttributeDefinition;
	using ObjectTypeAttributeDefinition = org.jboss.@as.controller.ObjectTypeAttributeDefinition;
	using ModelDescriptionConstants = org.jboss.@as.controller.descriptions.ModelDescriptionConstants;
	using ResourceDescriptionResolver = org.jboss.@as.controller.descriptions.ResourceDescriptionResolver;
	using ObjectTypeValidator = org.jboss.@as.controller.operations.validation.ObjectTypeValidator;
	using ModelNode = org.jboss.dmr.ModelNode;
	using ModelType = org.jboss.dmr.ModelType;


	/// <summary>
	/// Fix value type validation for ObjectTypeAttributeDefinition containing a map as value type.
	/// Works without this hack in WF-10, not in WF-8.
	/// 
	/// @author Christian Lipphardt
	/// </summary>
	public class FixedObjectTypeAttributeDefinition : ObjectTypeAttributeDefinition
	{

	  public FixedObjectTypeAttributeDefinition<T1>(AbstractAttributeDefinitionBuilder<T1> builder, string suffix, AttributeDefinition[] valueTypes) where T1 : org.jboss.@as.controller.ObjectTypeAttributeDefinition : base(builder, suffix, valueTypes)
	  {
	  }

	  protected internal override void addValueTypeDescription(ModelNode node, string prefix, ResourceBundle bundle, ResourceDescriptionResolver resolver, Locale locale)
	  {
		base.addValueTypeDescription(node, prefix, bundle, resolver, locale);

		try
		{
		  System.Reflection.FieldInfo valueTypesField = typeof(ObjectTypeAttributeDefinition).getDeclaredField("valueTypes");
		  valueTypesField.Accessible = true;
		  object value = valueTypesField.get(this);
		  if (value == null)
		  {
			return;
		  }
		  else if (value.GetType().IsAssignableFrom(typeof(AttributeDefinition[])))
		  {
			foreach (AttributeDefinition valueType in (AttributeDefinition[])value)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.dmr.ModelNode childType = node.get(org.jboss.as.controller.descriptions.ModelDescriptionConstants.VALUE_TYPE, valueType.getName());
			  ModelNode childType = node.get(ModelDescriptionConstants.VALUE_TYPE, valueType.Name);

			  if (valueType is MapAttributeDefinition)
			  {
				if (!childType.hasDefined(ModelDescriptionConstants.VALUE_TYPE))
				{
				  childType.get(ModelDescriptionConstants.VALUE_TYPE).set(ModelType.STRING);
				}
				if (!childType.hasDefined(ModelDescriptionConstants.EXPRESSIONS_ALLOWED))
				{
				  childType.get(ModelDescriptionConstants.EXPRESSIONS_ALLOWED).set(new ModelNode(false));
				}
			  }
			}
		  }
		}
		catch (NoSuchFieldException e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
		catch (IllegalAccessException e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
	  }

	  public sealed class Builder : AbstractAttributeDefinitionBuilder<Builder, FixedObjectTypeAttributeDefinition>
	  {
		internal string suffix;
		internal readonly AttributeDefinition[] valueTypes;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Builder(final String name, final org.jboss.as.controller.AttributeDefinition... valueTypes)
		public Builder(string name, params AttributeDefinition[] valueTypes) : base(name, ModelType.OBJECT, true)
		{
		  this.valueTypes = valueTypes;

		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static Builder of(final String name, final org.jboss.as.controller.AttributeDefinition... valueTypes)
		public static Builder of(string name, params AttributeDefinition[] valueTypes)
		{
		  return new Builder(name, valueTypes);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static Builder of(final String name, final org.jboss.as.controller.AttributeDefinition[] valueTypes, final org.jboss.as.controller.AttributeDefinition[] moreValueTypes)
		public static Builder of(string name, AttributeDefinition[] valueTypes, AttributeDefinition[] moreValueTypes)
		{
		  List<AttributeDefinition> list = new List<AttributeDefinition>(Arrays.asList(valueTypes));
		  list.AddRange(Arrays.asList(moreValueTypes));
		  AttributeDefinition[] allValueTypes = new AttributeDefinition[list.Count];
		  list.toArray(allValueTypes);

		  return new Builder(name, allValueTypes);
		}

		public FixedObjectTypeAttributeDefinition build()
		{
		  if (validator == null)
		  {
			  validator = new ObjectTypeValidator(allowNull, valueTypes);
		  }
	//      attributeMarshaller = new Object
		  return new FixedObjectTypeAttributeDefinition(this, suffix, valueTypes);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Builder setSuffix(final String suffix)
		public Builder setSuffix(string suffix)
		{
		  this.suffix = suffix;
		  return this;
		}

		/*
	   --------------------------
	   added for binary compatibility for running compatibilty tests
		*/
		public override Builder setAllowNull(bool allowNull)
		{
		  return base.setAllowNull(allowNull);
		}
	  }

	}

}