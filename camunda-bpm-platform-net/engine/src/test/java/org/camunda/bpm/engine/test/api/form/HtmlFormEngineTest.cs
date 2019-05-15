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
namespace org.camunda.bpm.engine.test.api.form
{

	using FormEngine = org.camunda.bpm.engine.impl.form.engine.FormEngine;
	using HtmlDocumentBuilder = org.camunda.bpm.engine.impl.form.engine.HtmlDocumentBuilder;
	using HtmlElementWriter = org.camunda.bpm.engine.impl.form.engine.HtmlElementWriter;
	using HtmlFormEngine = org.camunda.bpm.engine.impl.form.engine.HtmlFormEngine;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HtmlFormEngineTest : PluggableProcessEngineTestCase
	{

	  public virtual void testIsDefaultFormEngine()
	  {

		// make sure the html form engine is the default form engine:
		IDictionary<string, FormEngine> formEngines = processEngineConfiguration.FormEngines;
		assertTrue(formEngines[null] is HtmlFormEngine);

	  }

	  public virtual void testTransformNullFormData()
	  {
		HtmlFormEngine formEngine = new HtmlFormEngine();
		assertNull(formEngine.renderStartForm(null));
		assertNull(formEngine.renderTaskForm(null));
	  }

	  public virtual void testHtmlElementWriter()
	  {

		string htmlString = (new HtmlDocumentBuilder(new HtmlElementWriter("someTagName"))).endElement().HtmlString;
		assertHtmlEquals("<someTagName></someTagName>", htmlString);

		htmlString = (new HtmlDocumentBuilder(new HtmlElementWriter("someTagName", true))).endElement().HtmlString;
		assertHtmlEquals("<someTagName />", htmlString);

		htmlString = (new HtmlDocumentBuilder((new HtmlElementWriter("someTagName", true)).attribute("someAttr", "someAttrValue"))).endElement().HtmlString;
		assertHtmlEquals("<someTagName someAttr=\"someAttrValue\" />", htmlString);

		htmlString = (new HtmlDocumentBuilder((new HtmlElementWriter("someTagName")).attribute("someAttr", "someAttrValue"))).endElement().HtmlString;
		assertHtmlEquals("<someTagName someAttr=\"someAttrValue\"></someTagName>", htmlString);

		htmlString = (new HtmlDocumentBuilder((new HtmlElementWriter("someTagName")).attribute("someAttr", null))).endElement().HtmlString;
		assertHtmlEquals("<someTagName someAttr></someTagName>", htmlString);

		htmlString = (new HtmlDocumentBuilder((new HtmlElementWriter("someTagName")).textContent("someTextContent"))).endElement().HtmlString;
		assertHtmlEquals("<someTagName>someTextContent</someTagName>", htmlString);

		htmlString = (new HtmlDocumentBuilder(new HtmlElementWriter("someTagName"))).startElement(new HtmlElementWriter("someChildTag")).endElement().endElement().HtmlString;
		assertHtmlEquals("<someTagName><someChildTag></someChildTag></someTagName>", htmlString);

		htmlString = (new HtmlDocumentBuilder(new HtmlElementWriter("someTagName"))).startElement((new HtmlElementWriter("someChildTag")).textContent("someTextContent")).endElement().endElement().HtmlString;
		assertHtmlEquals("<someTagName><someChildTag>someTextContent</someChildTag></someTagName>", htmlString);

		htmlString = (new HtmlDocumentBuilder((new HtmlElementWriter("someTagName")).textContent("someTextContent"))).startElement(new HtmlElementWriter("someChildTag")).endElement().endElement().HtmlString;
		assertHtmlEquals("<someTagName><someChildTag></someChildTag>someTextContent</someTagName>", htmlString);

		// invalid usage

		try
		{
		  (new HtmlElementWriter("sometagname", true)).textContent("sometextcontet");
		}
		catch (System.InvalidOperationException e)
		{
		  assertTrue(e.Message.contains("Self-closing element cannot have text content"));
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRenderEmptyStartForm()
	  public virtual void testRenderEmptyStartForm()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		assertNull(formService.getRenderedStartForm(processDefinition.Id));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRenderStartForm()
	  public virtual void testRenderStartForm()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		string renderedForm = (string) formService.getRenderedStartForm(processDefinition.Id);

		string expectedForm = IoUtil.readClasspathResourceAsString("org/camunda/bpm/engine/test/api/form/HtmlFormEngineTest.testRenderStartForm.html");

		assertHtmlEquals(expectedForm, renderedForm);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRenderEnumField()
	  public virtual void testRenderEnumField()
	  {

		runtimeService.startProcessInstanceByKey("HtmlFormEngineTest.testRenderEnumField");

		Task t = taskService.createTaskQuery().singleResult();

		string renderedForm = (string) formService.getRenderedTaskForm(t.Id);

		string expectedForm = IoUtil.readClasspathResourceAsString("org/camunda/bpm/engine/test/api/form/HtmlFormEngineTest.testRenderEnumField.html");

		assertHtmlEquals(expectedForm, renderedForm);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRenderTaskForm()
	  public virtual void testRenderTaskForm()
	  {

		runtimeService.startProcessInstanceByKey("HtmlFormEngineTest.testRenderTaskForm");

		Task t = taskService.createTaskQuery().singleResult();

		string renderedForm = (string) formService.getRenderedTaskForm(t.Id);

		string expectedForm = IoUtil.readClasspathResourceAsString("org/camunda/bpm/engine/test/api/form/HtmlFormEngineTest.testRenderTaskForm.html");

		assertHtmlEquals(expectedForm, renderedForm);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRenderDateField()
	  public virtual void testRenderDateField()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		string renderedForm = (string) formService.getRenderedStartForm(processDefinition.Id);

		string expectedForm = IoUtil.readClasspathResourceAsString("org/camunda/bpm/engine/test/api/form/HtmlFormEngineTest.testRenderDateField.html");

		assertHtmlEquals(expectedForm, renderedForm);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRenderDateFieldWithPattern()
	  public virtual void testRenderDateFieldWithPattern()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		string renderedForm = (string) formService.getRenderedStartForm(processDefinition.Id);

		string expectedForm = IoUtil.readClasspathResourceAsString("org/camunda/bpm/engine/test/api/form/HtmlFormEngineTest.testRenderDateFieldWithPattern.html");

		assertHtmlEquals(expectedForm, renderedForm);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLegacyFormPropertySupport()
	  public virtual void testLegacyFormPropertySupport()
	  {

		runtimeService.startProcessInstanceByKey("HtmlFormEngineTest.testLegacyFormPropertySupport");

		Task t = taskService.createTaskQuery().singleResult();

		string renderedForm = (string) formService.getRenderedTaskForm(t.Id);

		string expectedForm = IoUtil.readClasspathResourceAsString("org/camunda/bpm/engine/test/api/form/HtmlFormEngineTest.testLegacyFormPropertySupport.html");

		assertHtmlEquals(expectedForm, renderedForm);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLegacyFormPropertySupportReadOnly()
	  public virtual void testLegacyFormPropertySupportReadOnly()
	  {

		runtimeService.startProcessInstanceByKey("HtmlFormEngineTest.testLegacyFormPropertySupportReadOnly");

		Task t = taskService.createTaskQuery().singleResult();

		string renderedForm = (string) formService.getRenderedTaskForm(t.Id);

		string expectedForm = IoUtil.readClasspathResourceAsString("org/camunda/bpm/engine/test/api/form/HtmlFormEngineTest.testLegacyFormPropertySupportReadOnly.html");

		assertHtmlEquals(expectedForm, renderedForm);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLegacyFormPropertySupportRequired()
	  public virtual void testLegacyFormPropertySupportRequired()
	  {

		runtimeService.startProcessInstanceByKey("HtmlFormEngineTest.testLegacyFormPropertySupportRequired");

		Task t = taskService.createTaskQuery().singleResult();

		string renderedForm = (string) formService.getRenderedTaskForm(t.Id);

		string expectedForm = IoUtil.readClasspathResourceAsString("org/camunda/bpm/engine/test/api/form/HtmlFormEngineTest.testLegacyFormPropertySupportRequired.html");

		assertHtmlEquals(expectedForm, renderedForm);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBusinessKey()
	  public virtual void testBusinessKey()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		string renderedForm = (string) formService.getRenderedStartForm(processDefinition.Id);

		string expectedForm = IoUtil.readClasspathResourceAsString("org/camunda/bpm/engine/test/api/form/HtmlFormEngineTest.testBusinessKey.html");

		assertHtmlEquals(expectedForm, renderedForm);

	  }

	  public virtual void assertHtmlEquals(string expected, string actual)
	  {
		assertEquals(filterWhitespace(expected), filterWhitespace(actual));
	  }

	  protected internal virtual string filterWhitespace(string tofilter)
	  {
		return tofilter.replaceAll("\\n", "").replaceAll("\\s", "");
	  }

	}

}