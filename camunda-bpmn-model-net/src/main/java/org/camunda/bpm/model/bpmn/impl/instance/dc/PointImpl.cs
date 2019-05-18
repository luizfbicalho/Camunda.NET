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
	using Point = org.camunda.bpm.model.bpmn.instance.dc.Point;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The DC point element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class PointImpl : BpmnModelElementInstanceImpl, Point
	{

	  protected internal static Attribute<double> xAttribute;
	  protected internal static Attribute<double> yAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(Point), DC_ELEMENT_POINT).namespaceUri(DC_NS).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		xAttribute = typeBuilder.doubleAttribute(DC_ATTRIBUTE_X).required().build();

		yAttribute = typeBuilder.doubleAttribute(DC_ATTRIBUTE_Y).required().build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<Point>
	  {
		  public Point newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new PointImpl(instanceContext);
		  }
	  }

	  public PointImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual double? getX()
	  {
		return xAttribute.getValue(this);
	  }

	  public virtual void setX(double x)
	  {
		xAttribute.setValue(this, x);
	  }

	  public virtual double? getY()
	  {
		return yAttribute.getValue(this);
	  }

	  public virtual void setY(double y)
	  {
		yAttribute.setValue(this, y);
	  }
	}

}