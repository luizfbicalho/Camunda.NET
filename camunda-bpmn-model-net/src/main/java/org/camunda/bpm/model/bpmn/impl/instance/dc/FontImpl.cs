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
namespace org.camunda.bpm.model.bpmn.impl.instance.dc
{
	using Font = org.camunda.bpm.model.bpmn.instance.dc.Font;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The DC font element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class FontImpl : BpmnModelElementInstanceImpl, Font
	{

	  protected internal static Attribute<string> nameAttribute;
	  protected internal static Attribute<double> sizeAttribute;
	  protected internal static Attribute<bool> isBoldAttribute;
	  protected internal static Attribute<bool> isItalicAttribute;
	  protected internal static Attribute<bool> isUnderlineAttribute;
	  protected internal static Attribute<bool> isStrikeTroughAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Font), DC_ELEMENT_FONT).namespaceUri(DC_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		nameAttribute = typeBuilder.stringAttribute(DC_ATTRIBUTE_NAME).build();

		sizeAttribute = typeBuilder.doubleAttribute(DC_ATTRIBUTE_SIZE).build();

		isBoldAttribute = typeBuilder.booleanAttribute(DC_ATTRIBUTE_IS_BOLD).build();

		isItalicAttribute = typeBuilder.booleanAttribute(DC_ATTRIBUTE_IS_ITALIC).build();

		isUnderlineAttribute = typeBuilder.booleanAttribute(DC_ATTRIBUTE_IS_UNDERLINE).build();

		isStrikeTroughAttribute = typeBuilder.booleanAttribute(DC_ATTRIBUTE_IS_STRIKE_THROUGH).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<Font>
	  {
		  public Font newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new FontImpl(instanceContext);
		  }
	  }

	  public FontImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Name
	  {
		  get
		  {
			return nameAttribute.getValue(this);
		  }
		  set
		  {
			nameAttribute.setValue(this, value);
		  }
	  }


	  public virtual double? Size
	  {
		  get
		  {
			return sizeAttribute.getValue(this);
		  }
		  set
		  {
			sizeAttribute.setValue(this, value);
		  }
	  }


	  public virtual bool? isBold()
	  {
		return isBoldAttribute.getValue(this);
	  }

	  public virtual void setBold(bool isBold)
	  {
		isBoldAttribute.setValue(this, isBold);
	  }

	  public virtual bool? isItalic()
	  {
		return isItalicAttribute.getValue(this);
	  }

	  public virtual void setItalic(bool isItalic)
	  {
		isItalicAttribute.setValue(this, isItalic);
	  }

	  public virtual bool? Underline
	  {
		  get
		  {
			return isUnderlineAttribute.getValue(this);
		  }
	  }

	  public virtual void SetUnderline(bool isUnderline)
	  {
		isUnderlineAttribute.setValue(this, isUnderline);
	  }

	  public virtual bool? StrikeThrough
	  {
		  get
		  {
			return isStrikeTroughAttribute.getValue(this);
		  }
	  }

	  public virtual bool StrikeTrough
	  {
		  set
		  {
			isStrikeTroughAttribute.setValue(this, value);
		  }
	  }
	}

}