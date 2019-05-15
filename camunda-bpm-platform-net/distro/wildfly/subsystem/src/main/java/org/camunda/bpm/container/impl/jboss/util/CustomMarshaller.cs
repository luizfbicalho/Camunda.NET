using System;
using System.Collections.Generic;
using System.Diagnostics;

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
	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;
	using AttributeMarshaller = org.jboss.@as.controller.AttributeMarshaller;
	using DefaultAttributeMarshaller = org.jboss.@as.controller.DefaultAttributeMarshaller;
	using ObjectListAttributeDefinition = org.jboss.@as.controller.ObjectListAttributeDefinition;
	using ObjectTypeAttributeDefinition = org.jboss.@as.controller.ObjectTypeAttributeDefinition;
	using ModelNode = org.jboss.dmr.ModelNode;
	using Property = org.jboss.dmr.Property;

	public class CustomMarshaller
	{

	  /// <summary>
	  /// Obtain the 'valueTypes' of the ObjectTypeAttributeDefinition through reflection because they are private in Wildfly 8.
	  /// </summary>
	  public static AttributeDefinition[] getValueTypes(object instance, Type clazz)
	  {
		try
		{
		  if (clazz.IsAssignableFrom(typeof(ObjectTypeAttributeDefinition)))
		  {
			System.Reflection.FieldInfo valueTypesField = clazz.getDeclaredField("valueTypes");
			valueTypesField.Accessible = true;
			object value = valueTypesField.get(instance);
			if (value != null)
			{
			  if (value.GetType().IsAssignableFrom(typeof(AttributeDefinition[])))
			  {
				return (AttributeDefinition[]) value;
			  }
			}
			return (AttributeDefinition[]) value;
		  }
		}
		catch (Exception e)
		{
		  throw new Exception("Unable to get valueTypes.", e);
		}

		return null;
	  }

	  /// <summary>
	  /// Obtain the 'valueType' of the ObjectListAttributeDefinition through reflection because they are private in Wildfly 8.
	  /// </summary>
	  public static AttributeDefinition getValueType(object instance, Type clazz)
	  {
		try
		{
		  System.Reflection.FieldInfo valueTypesField = clazz.getDeclaredField("valueType");
		  valueTypesField.Accessible = true;
		  object value = valueTypesField.get(instance);
		  if (value != null)
		  {
			if (value.GetType().IsAssignableFrom(typeof(AttributeDefinition)))
			{
			  return (AttributeDefinition) value;
			}
		  }
		  return (AttributeDefinition) value;
		}
		catch (Exception e)
		{
		  throw new Exception("Unable to get valueType.", e);
		}
	  }

	  /// <summary>
	  /// Marshall the attribute as an element where attribute name is the element and value is the text content.
	  /// </summary>
	  private class AttributeAsElementMarshaller : DefaultAttributeMarshaller
	  {

		public override bool MarshallableAsElement
		{
			get
			{
			  return true;
			}
		}

	  }

	  /// <summary>
	  ///  Marshaller for properties.
	  /// </summary>
	  private class PropertiesAttributeMarshaller : DefaultAttributeMarshaller
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void marshallAsElement(org.jboss.as.controller.AttributeDefinition attribute, org.jboss.dmr.ModelNode resourceModel, boolean marshallDefault, javax.xml.stream.XMLStreamWriter writer) throws javax.xml.stream.XMLStreamException
		public override void marshallAsElement(AttributeDefinition attribute, ModelNode resourceModel, bool marshallDefault, XMLStreamWriter writer)
		{
		  resourceModel = resourceModel.get(attribute.XmlName);
		  writer.writeStartElement(attribute.Name);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.jboss.dmr.Property> properties = resourceModel.asPropertyList();
		  IList<Property> properties = resourceModel.asPropertyList();
		  foreach (Property property in properties)
		  {
			writer.writeStartElement(org.jboss.@as.controller.parsing.Element.PROPERTY.LocalName);
			writer.writeAttribute(org.jboss.@as.controller.parsing.Attribute.NAME.LocalName, property.Name);
			writer.writeCharacters(property.Value.asString());
			writer.writeEndElement();
		  }
		  writer.writeEndElement();
		}

		public override bool MarshallableAsElement
		{
			get
			{
			  return true;
			}
		}

	  }

	  /// <summary>
	  /// Marshall the plugin object.
	  /// </summary>
	  private class PluginObjectTypeMarshaller : DefaultAttributeMarshaller
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void marshallAsElement(org.jboss.as.controller.AttributeDefinition attribute, org.jboss.dmr.ModelNode resourceModel, boolean marshallDefault, javax.xml.stream.XMLStreamWriter writer) throws javax.xml.stream.XMLStreamException
		public override void marshallAsElement(AttributeDefinition attribute, ModelNode resourceModel, bool marshallDefault, XMLStreamWriter writer)
		{

		  if (attribute is ObjectListAttributeDefinition)
		  {
			attribute = getValueType(attribute, typeof(ObjectListAttributeDefinition));
		  }

		  if (!(attribute is ObjectTypeAttributeDefinition))
		  {
			throw new XMLStreamException(string.Format("Attribute of class {0} is expected, but {1} received", "ObjectTypeAttributeDefinition", attribute.GetType().Name));
		  }

		  AttributeDefinition[] valueTypes;
		  valueTypes = CustomMarshaller.getValueTypes(attribute, typeof(ObjectTypeAttributeDefinition));

		  writer.writeStartElement(attribute.XmlName);
		  foreach (AttributeDefinition valueType in valueTypes)
		  {
			valueType.marshallAsElement(resourceModel, marshallDefault, writer);
		  }
		  writer.writeEndElement();
		}

		public override bool MarshallableAsElement
		{
			get
			{
			  return true;
			}
		}
	  }

	  /// <summary>
	  /// Marshall a list of objects.
	  /// </summary>
	  private class ObjectListMarshaller : AttributeMarshaller
	  {
		internal ObjectListMarshaller()
		{
		}

		public override bool MarshallableAsElement
		{
			get
			{
			  return true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void marshallAsElement(org.jboss.as.controller.AttributeDefinition attribute, org.jboss.dmr.ModelNode resourceModel, boolean marshallDefault, javax.xml.stream.XMLStreamWriter writer) throws javax.xml.stream.XMLStreamException
		public override void marshallAsElement(AttributeDefinition attribute, ModelNode resourceModel, bool marshallDefault, XMLStreamWriter writer)
		{
		  Debug.Assert(attribute is ObjectListAttributeDefinition);
		  ObjectListAttributeDefinition list = ((ObjectListAttributeDefinition) attribute);

		  ObjectTypeAttributeDefinition objectType = (ObjectTypeAttributeDefinition) CustomMarshaller.getValueType(list, typeof(ObjectListAttributeDefinition));
		  AttributeDefinition[] valueTypes = CustomMarshaller.getValueTypes(list, typeof(ObjectTypeAttributeDefinition));

		  if (resourceModel.hasDefined(attribute.Name))
		  {
			writer.writeStartElement(attribute.XmlName);
			foreach (ModelNode element in resourceModel.get(attribute.Name).asList())
			{
			  writer.writeStartElement(objectType.XmlName);
			  foreach (AttributeDefinition valueType in valueTypes)
			  {
				valueType.AttributeMarshaller.marshallAsElement(valueType, element, false, writer);
			  }
			  writer.writeEndElement();
			}
			writer.writeEndElement();
		  }
		}
	  }

	  public static readonly AttributeAsElementMarshaller ATTRIBUTE_AS_ELEMENT = new AttributeAsElementMarshaller();
	  public static readonly PluginObjectTypeMarshaller OBJECT_AS_ELEMENT = new PluginObjectTypeMarshaller();
	  public static readonly ObjectListMarshaller OBJECT_LIST = new ObjectListMarshaller();
	  public static readonly PropertiesAttributeMarshaller PROPERTIES_MARSHALLER = new PropertiesAttributeMarshaller();
	}

}