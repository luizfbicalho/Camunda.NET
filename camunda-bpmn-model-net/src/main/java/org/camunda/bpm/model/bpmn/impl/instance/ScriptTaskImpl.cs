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
namespace org.camunda.bpm.model.bpmn.impl.instance
{
	using ScriptTaskBuilder = org.camunda.bpm.model.bpmn.builder.ScriptTaskBuilder;
	using Script = org.camunda.bpm.model.bpmn.instance.Script;
	using ScriptTask = org.camunda.bpm.model.bpmn.instance.ScriptTask;
	using Task = org.camunda.bpm.model.bpmn.instance.Task;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

	using static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN scriptTask element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ScriptTaskImpl : TaskImpl, ScriptTask
	{

	  protected internal static Attribute<string> scriptFormatAttribute;
	  protected internal static ChildElement<Script> scriptChild;

	  /// <summary>
	  /// camunda extensions </summary>

	  protected internal static Attribute<string> camundaResultVariableAttribute;
	  protected internal static Attribute<string> camundaResourceAttribute;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ScriptTask), BPMN_ELEMENT_SCRIPT_TASK).namespaceUri(BPMN20_NS).extendsType(typeof(Task)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		scriptFormatAttribute = typeBuilder.stringAttribute(BPMN_ATTRIBUTE_SCRIPT_FORMAT).build();

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		scriptChild = sequenceBuilder.element(typeof(Script)).build();

		/// <summary>
		/// camunda extensions </summary>

		camundaResultVariableAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_RESULT_VARIABLE).@namespace(CAMUNDA_NS).build();

		camundaResourceAttribute = typeBuilder.stringAttribute(CAMUNDA_ATTRIBUTE_RESOURCE).@namespace(CAMUNDA_NS).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<ScriptTask>
	  {
		  public ScriptTask newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ScriptTaskImpl(instanceContext);
		  }
	  }

	  public ScriptTaskImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public override ScriptTaskBuilder builder()
	  {
		return new ScriptTaskBuilder((BpmnModelInstance) modelInstance, this);
	  }

	  public virtual string ScriptFormat
	  {
		  get
		  {
			return scriptFormatAttribute.getValue(this);
		  }
		  set
		  {
			scriptFormatAttribute.setValue(this, value);
		  }
	  }


	  public virtual Script Script
	  {
		  get
		  {
			return scriptChild.getChild(this);
		  }
		  set
		  {
			scriptChild.setChild(this, value);
		  }
	  }


	  /// <summary>
	  /// camunda extensions </summary>

	  public virtual string CamundaResultVariable
	  {
		  get
		  {
			return camundaResultVariableAttribute.getValue(this);
		  }
		  set
		  {
			camundaResultVariableAttribute.setValue(this, value);
		  }
	  }


	  public virtual string CamundaResource
	  {
		  get
		  {
			return camundaResourceAttribute.getValue(this);
		  }
		  set
		  {
			camundaResourceAttribute.setValue(this, value);
		  }
	  }


	}

}