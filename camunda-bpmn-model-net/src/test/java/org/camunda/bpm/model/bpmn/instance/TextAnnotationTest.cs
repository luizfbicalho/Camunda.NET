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
namespace org.camunda.bpm.model.bpmn.instance
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using BeforeClass = org.junit.BeforeClass;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Filip Hrisafov
	/// </summary>
	public class TextAnnotationTest : BpmnModelElementInstanceTest
	{

	  protected internal static BpmnModelInstance modelInstance;

	  public virtual TypeAssumption TypeAssumption
	  {
		  get
		  {
			return new TypeAssumption(typeof(Artifact), false);
		  }
	  }

	  public virtual ICollection<ChildElementAssumption> ChildElementAssumptions
	  {
		  get
		  {
			return Arrays.asList(new ChildElementAssumption(typeof(Text), 0, 1)
		   );
		  }
	  }

	  public virtual ICollection<AttributeAssumption> AttributesAssumptions
	  {
		  get
		  {
			return Arrays.asList(new AttributeAssumption("textFormat", false, false, "text/plain")
		   );
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void parseModel()
	  public static void parseModel()
	  {
		modelInstance = Bpmn.readModelFromStream(typeof(TextAnnotationTest).getResourceAsStream("TextAnnotationTest.bpmn"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTextAnnotationsByType()
	  public virtual void testGetTextAnnotationsByType()
	  {
		ICollection<TextAnnotation> textAnnotations = modelInstance.getModelElementsByType(typeof(TextAnnotation));
		assertThat(textAnnotations).NotNull.hasSize(2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTextAnnotationById()
	  public virtual void testGetTextAnnotationById()
	  {
		TextAnnotation textAnnotation = modelInstance.getModelElementById("textAnnotation2");
		assertThat(textAnnotation).NotNull;
		assertThat(textAnnotation.TextFormat).isEqualTo("text/plain");
		Text text = textAnnotation.Text;
		assertThat(text.TextContent).isEqualTo("Attached text annotation");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTextAnnotationAsAssociationSource()
	  public virtual void testTextAnnotationAsAssociationSource()
	  {
		Association association = modelInstance.getModelElementById("Association_1");
		BaseElement source = association.Source;
		assertThat(source).isInstanceOf(typeof(TextAnnotation));
		assertThat(source.Id).isEqualTo("textAnnotation2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTextAnnotationAsAssociationTarget()
	  public virtual void testTextAnnotationAsAssociationTarget()
	  {
		Association association = modelInstance.getModelElementById("Association_2");
		BaseElement target = association.Target;
		assertThat(target).isInstanceOf(typeof(TextAnnotation));
		assertThat(target.Id).isEqualTo("textAnnotation1");
	  }

	}

}