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
namespace org.camunda.bpm.model.xml.testmodel.instance
{
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ModelElementTypeBuilder_ModelTypeInstanceProvider = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder_ModelTypeInstanceProvider;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.MODEL_NAMESPACE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.TYPE_NAME_CHILD_RELATIONSHIP_DEFINITION;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ChildRelationshipDefinition : RelationshipDefinition
	{

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(ChildRelationshipDefinition), TYPE_NAME_CHILD_RELATIONSHIP_DEFINITION).namespaceUri(MODEL_NAMESPACE).extendsType(typeof(RelationshipDefinition)).instanceProvider(new ModelElementTypeBuilder_ModelTypeInstanceProviderAnonymousInnerClass());

		typeBuilder.build();
	  }

	  private class ModelElementTypeBuilder_ModelTypeInstanceProviderAnonymousInnerClass : ModelElementTypeBuilder_ModelTypeInstanceProvider<ChildRelationshipDefinition>
	  {

		  public ChildRelationshipDefinition newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new ChildRelationshipDefinition(instanceContext);
		  }
	  }

	  public ChildRelationshipDefinition(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }
	}

}