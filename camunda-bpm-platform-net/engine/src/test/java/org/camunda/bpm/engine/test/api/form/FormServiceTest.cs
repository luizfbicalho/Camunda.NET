using System;
using System.Collections.Generic;
using System.IO;

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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.booleanValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.createVariables;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.objectValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.serializedObjectValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.stringValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using FormField = org.camunda.bpm.engine.form.FormField;
	using FormProperty = org.camunda.bpm.engine.form.FormProperty;
	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using AbstractFormFieldType = org.camunda.bpm.engine.impl.form.type.AbstractFormFieldType;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using IoUtil = org.camunda.commons.utils.IoUtil;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// @author Tom Baeyens
	/// @author Falko Menge (camunda)
	/// </summary>
	public class FormServiceTest
	{
		private bool InstanceFieldsInitialized = false;

		public FormServiceTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.JavaSerializationFormatEnabled = true;
			return configuration;
		  }
	  }
	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  private RuntimeService runtimeService;
	  private TaskService taskService;
	  private RepositoryService repositoryService;
	  private HistoryService historyService;
	  private IdentityService identityService;
	  private FormService formService;
	  private CaseService caseService;
	  private ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		repositoryService = engineRule.RepositoryService;
		historyService = engineRule.HistoryService;
		formService = engineRule.FormService;
		caseService = engineRule.CaseService;
		identityService = engineRule.IdentityService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		identityService.saveUser(identityService.newUser("fozzie"));
		identityService.saveGroup(identityService.newGroup("management"));
		identityService.createMembership("fozzie", "management");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		identityService.deleteGroup("management");
		identityService.deleteUser("fozzie");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/util/approve.form", "org/camunda/bpm/engine/test/api/form/util/request.form", "org/camunda/bpm/engine/test/api/form/util/adjustRequest.form" }) @Test public void testGetStartFormByProcessDefinitionId()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/util/approve.form", "org/camunda/bpm/engine/test/api/form/util/request.form", "org/camunda/bpm/engine/test/api/form/util/adjustRequest.form" })]
	  public virtual void testGetStartFormByProcessDefinitionId()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		assertEquals(1, processDefinitions.Count);
		ProcessDefinition processDefinition = processDefinitions[0];

		object startForm = formService.getRenderedStartForm(processDefinition.Id, "juel");
		assertNotNull(startForm);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) @Test public void testGetStartFormByProcessDefinitionIdWithoutStartform()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testGetStartFormByProcessDefinitionIdWithoutStartform()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		assertEquals(1, processDefinitions.Count);
		ProcessDefinition processDefinition = processDefinitions[0];

		object startForm = formService.getRenderedStartForm(processDefinition.Id);
		assertNull(startForm);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormByKeyNullKey()
	  public virtual void testGetStartFormByKeyNullKey()
	  {
		try
		{
		  formService.getRenderedStartForm(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException)
		{
		  // Exception expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormByIdNullId()
	  public virtual void testGetStartFormByIdNullId()
	  {
		try
		{
		  formService.getRenderedStartForm(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException)
		{
		  // Exception expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormByIdUnexistingProcessDefinitionId()
	  public virtual void testGetStartFormByIdUnexistingProcessDefinitionId()
	  {
		try
		{
		  formService.getRenderedStartForm("unexistingId");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("no deployed process definition found with id", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormNullTaskId()
	  public virtual void testGetTaskFormNullTaskId()
	  {
		try
		{
		  formService.getRenderedTaskForm(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException)
		{
		  // Expected Exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormUnexistingTaskId()
	  public virtual void testGetTaskFormUnexistingTaskId()
	  {
		try
		{
		  formService.getRenderedTaskForm("unexistingtask");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("Task 'unexistingtask' not found", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskFormPropertyDefaultsAndFormRendering()
	  public virtual void testTaskFormPropertyDefaultsAndFormRendering()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String deploymentId = testRule.deploy("org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form", "org/camunda/bpm/engine/test/api/form/task.form").getId();
		string deploymentId = testRule.deploy("org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form", "org/camunda/bpm/engine/test/api/form/task.form").Id;

		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;
		StartFormData startForm = formService.getStartFormData(procDefId);
		assertNotNull(startForm);
		assertEquals(deploymentId, startForm.DeploymentId);
		assertEquals("org/camunda/bpm/engine/test/api/form/start.form", startForm.FormKey);
		assertEquals(new List<FormProperty>(), startForm.FormProperties);
		assertEquals(procDefId, startForm.ProcessDefinition.Id);

		object renderedStartForm = formService.getRenderedStartForm(procDefId, "juel");
		assertEquals("start form content", renderedStartForm);

		IDictionary<string, string> properties = new Dictionary<string, string>();
		properties["room"] = "5b";
		properties["speaker"] = "Mike";
		string processInstanceId = formService.submitStartFormData(procDefId, properties).Id;

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["room"] = "5b";
		expectedVariables["speaker"] = "Mike";

		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId);
		assertEquals(expectedVariables, variables);

		Task task = taskService.createTaskQuery().singleResult();
		string taskId = task.Id;
		TaskFormData taskForm = formService.getTaskFormData(taskId);
		assertEquals(deploymentId, taskForm.DeploymentId);
		assertEquals("org/camunda/bpm/engine/test/api/form/task.form", taskForm.FormKey);
		assertEquals(new List<FormProperty>(), taskForm.FormProperties);
		assertEquals(taskId, taskForm.Task.Id);

		assertEquals("Mike is speaking in room 5b", formService.getRenderedTaskForm(taskId, "juel"));

		properties = new Dictionary<>();
		properties["room"] = "3f";
		formService.submitTaskFormData(taskId, properties);

		expectedVariables = new Dictionary<>();
		expectedVariables["room"] = "3f";
		expectedVariables["speaker"] = "Mike";

		variables = runtimeService.getVariables(processInstanceId);
		assertEquals(expectedVariables, variables);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testFormPropertyHandlingDeprecated()
	  public virtual void testFormPropertyHandlingDeprecated()
	  {
		IDictionary<string, string> properties = new Dictionary<string, string>();
		properties["room"] = "5b"; // default
		properties["speaker"] = "Mike"; // variable name mapping
		properties["duration"] = "45"; // type conversion
		properties["free"] = "true"; // type conversion

		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;
		string processInstanceId = formService.submitStartFormData(procDefId, properties).Id;

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["room"] = "5b";
		expectedVariables["SpeakerName"] = "Mike";
		expectedVariables["duration"] = new long?(45);
		expectedVariables["free"] = true;

		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId);
		assertEquals(expectedVariables, variables);

		Address address = new Address();
		address.Street = "broadway";
		runtimeService.setVariable(processInstanceId, "address", address);

		string taskId = taskService.createTaskQuery().singleResult().Id;
		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		IList<FormProperty> formProperties = taskFormData.FormProperties;
		FormProperty propertyRoom = formProperties[0];
		assertEquals("room", propertyRoom.Id);
		assertEquals("5b", propertyRoom.Value);

		FormProperty propertyDuration = formProperties[1];
		assertEquals("duration", propertyDuration.Id);
		assertEquals("45", propertyDuration.Value);

		FormProperty propertySpeaker = formProperties[2];
		assertEquals("speaker", propertySpeaker.Id);
		assertEquals("Mike", propertySpeaker.Value);

		FormProperty propertyStreet = formProperties[3];
		assertEquals("street", propertyStreet.Id);
		assertEquals("broadway", propertyStreet.Value);

		FormProperty propertyFree = formProperties[4];
		assertEquals("free", propertyFree.Id);
		assertEquals("true", propertyFree.Value);

		assertEquals(5, formProperties.Count);

		try
		{
		  formService.submitTaskFormData(taskId, new Dictionary<string, string>());
		  fail("expected exception about required form property 'street'");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}

		try
		{
		  properties = new Dictionary<>();
		  properties["speaker"] = "its not allowed to update speaker!";
		  formService.submitTaskFormData(taskId, properties);
		  fail("expected exception about a non writable form property 'speaker'");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}

		properties = new Dictionary<>();
		properties["street"] = "rubensstraat";
		formService.submitTaskFormData(taskId, properties);

		expectedVariables = new Dictionary<>();
		expectedVariables["room"] = "5b";
		expectedVariables["SpeakerName"] = "Mike";
		expectedVariables["duration"] = new long?(45);
		expectedVariables["free"] = true;

		variables = runtimeService.getVariables(processInstanceId);
		address = (Address) variables.Remove("address");
		assertEquals("rubensstraat", address.Street);
		assertEquals(expectedVariables, variables);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testFormPropertyHandling()
	  public virtual void testFormPropertyHandling()
	  {
		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["room"] = "5b"; // default
		properties["speaker"] = "Mike"; // variable name mapping
		properties["duration"] = 45L; // type conversion
		properties["free"] = "true"; // type conversion

		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;
		string processInstanceId = formService.submitStartForm(procDefId, properties).Id;

		IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
		expectedVariables["room"] = "5b";
		expectedVariables["SpeakerName"] = "Mike";
		expectedVariables["duration"] = new long?(45);
		expectedVariables["free"] = true;

		IDictionary<string, object> variables = runtimeService.getVariables(processInstanceId);
		assertEquals(expectedVariables, variables);

		Address address = new Address();
		address.Street = "broadway";
		runtimeService.setVariable(processInstanceId, "address", address);

		string taskId = taskService.createTaskQuery().singleResult().Id;
		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		IList<FormProperty> formProperties = taskFormData.FormProperties;
		FormProperty propertyRoom = formProperties[0];
		assertEquals("room", propertyRoom.Id);
		assertEquals("5b", propertyRoom.Value);

		FormProperty propertyDuration = formProperties[1];
		assertEquals("duration", propertyDuration.Id);
		assertEquals("45", propertyDuration.Value);

		FormProperty propertySpeaker = formProperties[2];
		assertEquals("speaker", propertySpeaker.Id);
		assertEquals("Mike", propertySpeaker.Value);

		FormProperty propertyStreet = formProperties[3];
		assertEquals("street", propertyStreet.Id);
		assertEquals("broadway", propertyStreet.Value);

		FormProperty propertyFree = formProperties[4];
		assertEquals("free", propertyFree.Id);
		assertEquals("true", propertyFree.Value);

		assertEquals(5, formProperties.Count);

		try
		{
		  formService.submitTaskForm(taskId, new Dictionary<string, object>());
		  fail("expected exception about required form property 'street'");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}

		try
		{
		  properties = new Dictionary<>();
		  properties["speaker"] = "its not allowed to update speaker!";
		  formService.submitTaskForm(taskId, properties);
		  fail("expected exception about a non writable form property 'speaker'");
		}
		catch (ProcessEngineException)
		{
		  // OK
		}

		properties = new Dictionary<>();
		properties["street"] = "rubensstraat";
		formService.submitTaskForm(taskId, properties);

		expectedVariables = new Dictionary<>();
		expectedVariables["room"] = "5b";
		expectedVariables["SpeakerName"] = "Mike";
		expectedVariables["duration"] = new long?(45);
		expectedVariables["free"] = true;

		variables = runtimeService.getVariables(processInstanceId);
		address = (Address) variables.Remove("address");
		assertEquals("rubensstraat", address.Street);
		assertEquals(expectedVariables, variables);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Deployment @Test public void testFormPropertyDetails()
	  public virtual void testFormPropertyDetails()
	  {
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;
		StartFormData startFormData = formService.getStartFormData(procDefId);
		FormProperty property = startFormData.FormProperties[0];
		assertEquals("speaker", property.Id);
		assertNull(property.Value);
		assertTrue(property.Readable);
		assertTrue(property.Writable);
		assertFalse(property.Required);
		assertEquals("string", property.Type.Name);

		property = startFormData.FormProperties[1];
		assertEquals("start", property.Id);
		assertNull(property.Value);
		assertTrue(property.Readable);
		assertTrue(property.Writable);
		assertFalse(property.Required);
		assertEquals("date", property.Type.Name);
		assertEquals("dd-MMM-yyyy", property.Type.getInformation("datePattern"));

		property = startFormData.FormProperties[2];
		assertEquals("direction", property.Id);
		assertNull(property.Value);
		assertTrue(property.Readable);
		assertTrue(property.Writable);
		assertFalse(property.Required);
		assertEquals("enum", property.Type.Name);
		IDictionary<string, string> values = (IDictionary<string, string>) property.Type.getInformation("values");

		IDictionary<string, string> expectedValues = new LinkedHashMap<string, string>();
		expectedValues["left"] = "Go Left";
		expectedValues["right"] = "Go Right";
		expectedValues["up"] = "Go Up";
		expectedValues["down"] = "Go Down";

		// ACT-1023: check if ordering is retained
		IEnumerator<KeyValuePair<string, string>> expectedValuesIterator = expectedValues.SetOfKeyValuePairs().GetEnumerator();
		foreach (KeyValuePair<string, string> entry in values.SetOfKeyValuePairs())
		{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		  KeyValuePair<string, string> expectedEntryAtLocation = expectedValuesIterator.next();
		  assertEquals(expectedEntryAtLocation.Key, entry.Key);
		  assertEquals(expectedEntryAtLocation.Value, entry.Value);
		}
		assertEquals(expectedValues, values);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testInvalidFormKeyReference()
	  public virtual void testInvalidFormKeyReference()
	  {
		try
		{
		  formService.getRenderedStartForm(repositoryService.createProcessDefinitionQuery().singleResult().Id, "juel");
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  testRule.assertTextPresent("Form with formKey 'IDoNotExist' does not exist", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testSubmitStartFormDataWithBusinessKey()
	  public virtual void testSubmitStartFormDataWithBusinessKey()
	  {
		IDictionary<string, string> properties = new Dictionary<string, string>();
		properties["duration"] = "45";
		properties["speaker"] = "Mike";
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		ProcessInstance processInstance = formService.submitStartFormData(procDefId, "123", properties);
		assertEquals("123", processInstance.BusinessKey);

		assertEquals(processInstance.Id, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("123").singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml"}) @Test public void testSubmitStartFormDataTypedVariables()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml"})]
	  public virtual void testSubmitStartFormDataTypedVariables()
	  {
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		string stringValue = "some string";
		string serializedValue = "some value";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessInstance processInstance = formService.submitStartForm(procDefId, createVariables().putValueTyped("boolean", booleanValue(null)).putValueTyped("string", stringValue(stringValue)).putValueTyped("serializedObject", serializedObjectValue(serializedValue).objectTypeName(typeof(string).FullName).serializationDataFormat(Variables.SerializationDataFormats.JAVA).create()).putValueTyped("object", objectValue(serializedValue).create()));

		VariableMap variables = runtimeService.getVariablesTyped(processInstance.Id, false);
		assertEquals(booleanValue(null), variables.getValueTyped("boolean"));
		assertEquals(stringValue(stringValue), variables.getValueTyped("string"));
		assertNotNull(variables.getValueTyped<ObjectValue>("serializedObject").ValueSerialized);
		assertNotNull(variables.getValueTyped<ObjectValue>("object").ValueSerialized);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml"}) @Test public void testSubmitTaskFormDataTypedVariables()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml"})]
	  public virtual void testSubmitTaskFormDataTypedVariables()
	  {
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		ProcessInstance processInstance = formService.submitStartForm(procDefId, createVariables());

		Task task = taskService.createTaskQuery().singleResult();

		string stringValue = "some string";
		string serializedValue = "some value";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		formService.submitTaskForm(task.Id, createVariables().putValueTyped("boolean", booleanValue(null)).putValueTyped("string", stringValue(stringValue)).putValueTyped("serializedObject", serializedObjectValue(serializedValue).objectTypeName(typeof(string).FullName).serializationDataFormat(Variables.SerializationDataFormats.JAVA).create()).putValueTyped("object", objectValue(serializedValue).create()));

		VariableMap variables = runtimeService.getVariablesTyped(processInstance.Id, false);
		assertEquals(booleanValue(null), variables.getValueTyped("boolean"));
		assertEquals(stringValue(stringValue), variables.getValueTyped("string"));
		assertNotNull(variables.getValueTyped<ObjectValue>("serializedObject").ValueSerialized);
		assertNotNull(variables.getValueTyped<ObjectValue>("object").ValueSerialized);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml"}) @Test public void testSubmitFormVariablesNull()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml"})]
	  public virtual void testSubmitFormVariablesNull()
	  {
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		// assert that I can submit the start form with variables null
		formService.submitStartForm(procDefId, null);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		// assert that I can submit the task form with variables null
		formService.submitTaskForm(task.Id, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitTaskFormForStandaloneTask()
	  public virtual void testSubmitTaskFormForStandaloneTask()
	  {
		// given
		string id = "standaloneTask";
		Task task = taskService.newTask(id);
		taskService.saveTask(task);

		// when
		formService.submitTaskForm(task.Id, Variables.createVariables().putValue("foo", "bar"));


		if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT.Id)
		{
		  HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().taskIdIn(id).singleResult();

		  assertNotNull(variableInstance);
		  assertEquals("foo", variableInstance.Name);
		  assertEquals("bar", variableInstance.Value);
		}

		taskService.deleteTask(id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) @Test public void testSubmitTaskFormForCmmnHumanTask()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testSubmitTaskFormForCmmnHumanTask()
	  {
		caseService.createCaseInstanceByKey("oneTaskCase");

		Task task = taskService.createTaskQuery().singleResult();

		string stringValue = "some string";
		string serializedValue = "some value";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		formService.submitTaskForm(task.Id, createVariables().putValueTyped("boolean", booleanValue(null)).putValueTyped("string", stringValue(stringValue)).putValueTyped("serializedObject", serializedObjectValue(serializedValue).objectTypeName(typeof(string).FullName).serializationDataFormat(Variables.SerializationDataFormats.JAVA).create()).putValueTyped("object", objectValue(serializedValue).create()));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testSubmitStartFormWithBusinessKey()
	  public virtual void testSubmitStartFormWithBusinessKey()
	  {
		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["duration"] = 45L;
		properties["speaker"] = "Mike";
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		ProcessInstance processInstance = formService.submitStartForm(procDefId, "123", properties);
		assertEquals("123", processInstance.BusinessKey);

		assertEquals(processInstance.Id, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("123").singleResult().Id);
		IDictionary<string, object> variables = runtimeService.getVariables(processInstance.Id);
		assertEquals("Mike", variables["SpeakerName"]);
		assertEquals(45L, variables["duration"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testSubmitStartFormWithoutProperties()
	  public virtual void testSubmitStartFormWithoutProperties()
	  {
		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["duration"] = 45L;
		properties["speaker"] = "Mike";
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		ProcessInstance processInstance = formService.submitStartForm(procDefId, "123", properties);
		assertEquals("123", processInstance.BusinessKey);

		assertEquals(processInstance.Id, runtimeService.createProcessInstanceQuery().processInstanceBusinessKey("123").singleResult().Id);
		IDictionary<string, object> variables = runtimeService.getVariables(processInstance.Id);
		assertEquals("Mike", variables["speaker"]);
		assertEquals(45L, variables["duration"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormKeyEmptyArgument()
	  public virtual void testGetStartFormKeyEmptyArgument()
	  {
		try
		{
		  formService.getStartFormKey(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("The process definition id is mandatory, but 'null' has been provided.", ae.Message);
		}

		try
		{
		  formService.getStartFormKey("");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("The process definition id is mandatory, but '' has been provided.", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml") @Test public void testGetStartFormKey()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml")]
	  public virtual void testGetStartFormKey()
	  {
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;
		string expectedFormKey = formService.getStartFormData(processDefinitionId).FormKey;
		string actualFormKey = formService.getStartFormKey(processDefinitionId);
		assertEquals(expectedFormKey, actualFormKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormKeyEmptyArguments()
	  public virtual void testGetTaskFormKeyEmptyArguments()
	  {
		try
		{
		  formService.getTaskFormKey(null, "23");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("The process definition id is mandatory, but 'null' has been provided.", ae.Message);
		}

		try
		{
		  formService.getTaskFormKey("", "23");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("The process definition id is mandatory, but '' has been provided.", ae.Message);
		}

		try
		{
		  formService.getTaskFormKey("42", null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("The task definition key is mandatory, but 'null' has been provided.", ae.Message);
		}

		try
		{
		  formService.getTaskFormKey("42", "");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("The task definition key is mandatory, but '' has been provided.", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml") @Test public void testGetTaskFormKey()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/form/FormsProcess.bpmn20.xml")]
	  public virtual void testGetTaskFormKey()
	  {
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;
		runtimeService.startProcessInstanceById(processDefinitionId);
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		string expectedFormKey = formService.getTaskFormData(task.Id).FormKey;
		string actualFormKey = formService.getTaskFormKey(task.ProcessDefinitionId, task.TaskDefinitionKey);
		assertEquals(expectedFormKey, actualFormKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testGetTaskFormKeyWithExpression()
	  public virtual void testGetTaskFormKeyWithExpression()
	  {
		runtimeService.startProcessInstanceByKey("FormsProcess", CollectionUtil.singletonMap("dynamicKey", "test"));
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
		assertEquals("test", formService.getTaskFormData(task.Id).FormKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/form/FormServiceTest.startFormFields.bpmn20.xml"}) @Test public void testGetStartFormVariables()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/form/FormServiceTest.startFormFields.bpmn20.xml"})]
	  public virtual void testGetStartFormVariables()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		VariableMap variables = formService.getStartFormVariables(processDefinition.Id);
		assertEquals(4, variables.size());

		assertEquals("someString", variables.get("stringField"));
		assertEquals("someString", variables.getValueTyped("stringField").Value);
		assertEquals(ValueType.STRING, variables.getValueTyped("stringField").Type);

		assertEquals(5l, variables.get("longField"));
		assertEquals(5l, variables.getValueTyped("longField").Value);
		assertEquals(ValueType.LONG, variables.getValueTyped("longField").Type);

		assertNull(variables.get("customField"));
		assertNull(variables.getValueTyped("customField").Value);
		assertEquals(ValueType.STRING, variables.getValueTyped("customField").Type);

		assertNotNull(variables.get("dateField"));
		assertEquals(variables.get("dateField"), variables.getValueTyped("dateField").Value);
		assertEquals(ValueType.STRING, variables.getValueTyped("dateField").Type);

		AbstractFormFieldType dateFormType = processEngineConfiguration.FormTypes.getFormType("date");
		DateTime dateValue = (DateTime) dateFormType.convertToModelValue(variables.getValueTyped("dateField")).Value;
		DateTime calendar = new DateTime();
		calendar = new DateTime(dateValue);
		assertEquals(10, calendar.Day);
		assertEquals(1, calendar.Month);
		assertEquals(2013, calendar.Year);

		// get restricted set of variables:
		variables = formService.getStartFormVariables(processDefinition.Id, Arrays.asList("stringField"), true);
		assertEquals(1, variables.size());
		assertEquals("someString", variables.get("stringField"));
		assertEquals("someString", variables.getValueTyped("stringField").Value);
		assertEquals(ValueType.STRING, variables.getValueTyped("stringField").Type);

		// request non-existing variable
		variables = formService.getStartFormVariables(processDefinition.Id, Arrays.asList("non-existing!"), true);
		assertEquals(0, variables.size());

		// null => all
		variables = formService.getStartFormVariables(processDefinition.Id, null, true);
		assertEquals(4, variables.size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/form/FormServiceTest.startFormFieldsUnknownType.bpmn20.xml"}) @Test public void testGetStartFormVariablesEnumType()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/form/FormServiceTest.startFormFieldsUnknownType.bpmn20.xml"})]
	  public virtual void testGetStartFormVariablesEnumType()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		VariableMap startFormVariables = formService.getStartFormVariables(processDefinition.Id);
		assertEquals("a", startFormVariables.get("enumField"));
		assertEquals(ValueType.STRING, startFormVariables.getValueTyped("enumField").Type);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/form/FormServiceTest.taskFormFields.bpmn20.xml"}) @Test public void testGetTaskFormVariables()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/form/FormServiceTest.taskFormFields.bpmn20.xml"})]
	  public virtual void testGetTaskFormVariables()
	  {

		IDictionary<string, object> processVars = new Dictionary<string, object>();
		processVars["someString"] = "initialValue";
		processVars["initialBooleanVariable"] = true;
		processVars["initialLongVariable"] = 1l;
		processVars["serializable"] = Arrays.asList("a", "b", "c");

		runtimeService.startProcessInstanceByKey("testProcess", processVars);

		Task task = taskService.createTaskQuery().singleResult();
		VariableMap variables = formService.getTaskFormVariables(task.Id);
		assertEquals(7, variables.size());

		assertEquals("someString", variables.get("stringField"));
		assertEquals("someString", variables.getValueTyped("stringField").Value);
		assertEquals(ValueType.STRING, variables.getValueTyped("stringField").Type);

		assertEquals(5l, variables.get("longField"));
		assertEquals(5l, variables.getValueTyped("longField").Value);
		assertEquals(ValueType.LONG, variables.getValueTyped("longField").Type);

		assertNull(variables.get("customField"));
		assertNull(variables.getValueTyped("customField").Value);
		assertEquals(ValueType.STRING, variables.getValueTyped("customField").Type);

		assertEquals("initialValue", variables.get("someString"));
		assertEquals("initialValue", variables.getValueTyped("someString").Value);
		assertEquals(ValueType.STRING, variables.getValueTyped("someString").Type);

		assertEquals(true, variables.get("initialBooleanVariable"));
		assertEquals(true, variables.getValueTyped("initialBooleanVariable").Value);
		assertEquals(ValueType.BOOLEAN, variables.getValueTyped("initialBooleanVariable").Type);

		assertEquals(1l, variables.get("initialLongVariable"));
		assertEquals(1l, variables.getValueTyped("initialLongVariable").Value);
		assertEquals(ValueType.LONG, variables.getValueTyped("initialLongVariable").Type);

		assertNotNull(variables.get("serializable"));

		// override the long variable
		taskService.setVariableLocal(task.Id, "initialLongVariable", 2l);

		variables = formService.getTaskFormVariables(task.Id);
		assertEquals(7, variables.size());

		assertEquals(2l, variables.get("initialLongVariable"));
		assertEquals(2l, variables.getValueTyped("initialLongVariable").Value);
		assertEquals(ValueType.LONG, variables.getValueTyped("initialLongVariable").Type);

		// get restricted set of variables (form field):
		variables = formService.getTaskFormVariables(task.Id, Arrays.asList("someString"), true);
		assertEquals(1, variables.size());
		assertEquals("initialValue", variables.get("someString"));
		assertEquals("initialValue", variables.getValueTyped("someString").Value);
		assertEquals(ValueType.STRING, variables.getValueTyped("someString").Type);

		// get restricted set of variables (process variable):
		variables = formService.getTaskFormVariables(task.Id, Arrays.asList("initialBooleanVariable"), true);
		assertEquals(1, variables.size());
		assertEquals(true, variables.get("initialBooleanVariable"));
		assertEquals(true, variables.getValueTyped("initialBooleanVariable").Value);
		assertEquals(ValueType.BOOLEAN, variables.getValueTyped("initialBooleanVariable").Type);

		// request non-existing variable
		variables = formService.getTaskFormVariables(task.Id, Arrays.asList("non-existing!"), true);
		assertEquals(0, variables.size());

		// null => all
		variables = formService.getTaskFormVariables(task.Id, null, true);
		assertEquals(7, variables.size());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormVariables_StandaloneTask()
	  public virtual void testGetTaskFormVariables_StandaloneTask()
	  {

		IDictionary<string, object> processVars = new Dictionary<string, object>();
		processVars["someString"] = "initialValue";
		processVars["initialBooleanVariable"] = true;
		processVars["initialLongVariable"] = 1l;
		processVars["serializable"] = Arrays.asList("a", "b", "c");

		// create new standalone task
		Task standaloneTask = taskService.newTask();
		standaloneTask.Name = "A Standalone Task";
		taskService.saveTask(standaloneTask);

		Task task = taskService.createTaskQuery().singleResult();

		// set variables
		taskService.setVariables(task.Id, processVars);

		VariableMap variables = formService.getTaskFormVariables(task.Id);
		assertEquals(4, variables.size());

		assertEquals("initialValue", variables.get("someString"));
		assertEquals("initialValue", variables.getValueTyped("someString").Value);
		assertEquals(ValueType.STRING, variables.getValueTyped("someString").Type);

		assertEquals(true, variables.get("initialBooleanVariable"));
		assertEquals(true, variables.getValueTyped("initialBooleanVariable").Value);
		assertEquals(ValueType.BOOLEAN, variables.getValueTyped("initialBooleanVariable").Type);

		assertEquals(1l, variables.get("initialLongVariable"));
		assertEquals(1l, variables.getValueTyped("initialLongVariable").Value);
		assertEquals(ValueType.LONG, variables.getValueTyped("initialLongVariable").Type);

		assertNotNull(variables.get("serializable"));

		// override the long variable
		taskService.setVariable(task.Id, "initialLongVariable", 2l);

		variables = formService.getTaskFormVariables(task.Id);
		assertEquals(4, variables.size());

		assertEquals(2l, variables.get("initialLongVariable"));
		assertEquals(2l, variables.getValueTyped("initialLongVariable").Value);
		assertEquals(ValueType.LONG, variables.getValueTyped("initialLongVariable").Type);

		// get restricted set of variables
		variables = formService.getTaskFormVariables(task.Id, Arrays.asList("someString"), true);
		assertEquals(1, variables.size());
		assertEquals("initialValue", variables.get("someString"));
		assertEquals("initialValue", variables.getValueTyped("someString").Value);
		assertEquals(ValueType.STRING, variables.getValueTyped("someString").Type);

		// request non-existing variable
		variables = formService.getTaskFormVariables(task.Id, Arrays.asList("non-existing!"), true);
		assertEquals(0, variables.size());

		// null => all
		variables = formService.getTaskFormVariables(task.Id, null, true);
		assertEquals(4, variables.size());

		// Finally, delete task
		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) @Test public void testSubmitStartFormWithObjectVariables()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testSubmitStartFormWithObjectVariables()
	  {
		// given
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		// when a start form is submitted with an object variable
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = new List<string>();
		ProcessInstance processInstance = formService.submitStartForm(processDefinition.Id, variables);

		// then the variable is available as a process variable
		List<string> var = (List<string>) runtimeService.getVariable(processInstance.Id, "var");
		assertNotNull(var);
		assertTrue(var.Count == 0);

		// then no historic form property event has been written since this is not supported for custom objects
		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_FULL)
		{
		  assertEquals(0, historyService.createHistoricDetailQuery().formFields().count());
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml" }) @Test public void testSubmitTaskFormWithObjectVariables()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml" })]
	  public virtual void testSubmitTaskFormWithObjectVariables()
	  {
		// given
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("twoTasksProcess");

		// when a task form is submitted with an object variable
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["var"] = new List<string>();
		formService.submitTaskForm(task.Id, variables);

		// then the variable is available as a process variable
		List<string> var = (List<string>) runtimeService.getVariable(processInstance.Id, "var");
		assertNotNull(var);
		assertTrue(var.Count == 0);

		// then no historic form property event has been written since this is not supported for custom objects
		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_FULL)
		{
		  assertEquals(0, historyService.createHistoricDetailQuery().formFields().count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/task/TaskServiceTest.testCompleteTaskWithVariablesInReturn.bpmn20.xml" }) @Test public void testSubmitTaskFormWithVariablesInReturn()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/task/TaskServiceTest.testCompleteTaskWithVariablesInReturn.bpmn20.xml" })]
	  public virtual void testSubmitTaskFormWithVariablesInReturn()
	  {
		string processVarName = "processVar";
		string processVarValue = "processVarValue";

		string taskVarName = "taskVar";
		string taskVarValue = "taskVarValue";

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[processVarName] = processVarValue;

		runtimeService.startProcessInstanceByKey("TaskServiceTest.testCompleteTaskWithVariablesInReturn", variables);

		Task firstUserTask = taskService.createTaskQuery().taskName("First User Task").singleResult();
		taskService.setVariable(firstUserTask.Id, "x", 1);

		IDictionary<string, object> additionalVariables = new Dictionary<string, object>();
		additionalVariables[taskVarName] = taskVarValue;

		// After completion of firstUserTask a script Task sets 'x' = 5
		VariableMap vars = formService.submitTaskFormWithVariablesInReturn(firstUserTask.Id, additionalVariables, true);
		assertEquals(3, vars.size());
		assertEquals(5, vars.get("x"));
		assertEquals(ValueType.INTEGER, vars.getValueTyped("x").Type);
		assertEquals(processVarValue, vars.get(processVarName));
		assertEquals(ValueType.STRING, vars.getValueTyped(processVarName).Type);
		assertEquals(taskVarValue, vars.get(taskVarName));

		additionalVariables = new Dictionary<>();
		additionalVariables["x"] = 7;
		Task secondUserTask = taskService.createTaskQuery().taskName("Second User Task").singleResult();
		vars = formService.submitTaskFormWithVariablesInReturn(secondUserTask.Id, additionalVariables, true);
		assertEquals(3, vars.size());
		assertEquals(7, vars.get("x"));
		assertEquals(processVarValue, vars.get(processVarName));
		assertEquals(taskVarValue, vars.get(taskVarName));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/twoParallelTasksProcess.bpmn20.xml" }) @Test public void testSubmitTaskFormWithVariablesInReturnParallel()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/twoParallelTasksProcess.bpmn20.xml" })]
	  public virtual void testSubmitTaskFormWithVariablesInReturnParallel()
	  {
		string processVarName = "processVar";
		string processVarValue = "processVarValue";

		string task1VarName = "taskVar1";
		string task2VarName = "taskVar2";
		string task1VarValue = "taskVarValue1";
		string task2VarValue = "taskVarValue2";

		string additionalVar = "additionalVar";
		string additionalVarValue = "additionalVarValue";

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[processVarName] = processVarValue;
		runtimeService.startProcessInstanceByKey("twoParallelTasksProcess", variables);

		Task firstTask = taskService.createTaskQuery().taskName("First Task").singleResult();
		taskService.setVariable(firstTask.Id, task1VarName, task1VarValue);
		Task secondTask = taskService.createTaskQuery().taskName("Second Task").singleResult();
		taskService.setVariable(secondTask.Id, task2VarName, task2VarValue);

		IDictionary<string, object> vars = formService.submitTaskFormWithVariablesInReturn(firstTask.Id, null, true);

		assertEquals(3, vars.Count);
		assertEquals(processVarValue, vars[processVarName]);
		assertEquals(task1VarValue, vars[task1VarName]);
		assertEquals(task2VarValue, vars[task2VarName]);

		IDictionary<string, object> additionalVariables = new Dictionary<string, object>();
		additionalVariables[additionalVar] = additionalVarValue;

		vars = formService.submitTaskFormWithVariablesInReturn(secondTask.Id, additionalVariables, true);
		assertEquals(4, vars.Count);
		assertEquals(processVarValue, vars[processVarName]);
		assertEquals(task1VarValue, vars[task1VarName]);
		assertEquals(task2VarValue, vars[task2VarName]);
		assertEquals(additionalVarValue, vars[additionalVar]);
	  }

	  /// <summary>
	  /// Tests that the variablesInReturn logic is not applied
	  /// when we call the regular complete API. This is a performance optimization.
	  /// Loading all variables may be expensive.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSubmitTaskFormAndDoNotDeserializeVariables()
	  public virtual void testSubmitTaskFormAndDoNotDeserializeVariables()
	  {
		// given
		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().subProcess().embeddedSubProcess().startEvent().userTask("task1").userTask("task2").endEvent().subProcessDone().endEvent().done();

		testRule.deploy(process);

		runtimeService.startProcessInstanceByKey("process", Variables.putValue("var", "val"));

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().singleResult();
		Task task = taskService.createTaskQuery().singleResult();

		// when
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean hasLoadedAnyVariables = processEngineConfiguration.getCommandExecutorTxRequired().execute(new org.camunda.bpm.engine.impl.interceptor.Command<bool>()
		bool hasLoadedAnyVariables = processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, task));

		// then
		assertThat(hasLoadedAnyVariables).False;
	  }

	  private class CommandAnonymousInnerClass : Command<bool>
	  {
		  private readonly FormServiceTest outerInstance;

		  private Task task;

		  public CommandAnonymousInnerClass(FormServiceTest outerInstance, Task task)
		  {
			  this.outerInstance = outerInstance;
			  this.task = task;
		  }


		  public bool? execute(CommandContext commandContext)
		  {
			outerInstance.formService.submitTaskForm(task.Id, null);
			return commandContext.DbEntityManager.getCachedEntitiesByType(typeof(VariableInstanceEntity)).Count > 0;
		  }
	  }



//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml") public void testSubmitTaskFormWithVarialbesInReturnShouldDeserializeObjectValue()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml")]
	  public virtual void testSubmitTaskFormWithVarialbesInReturnShouldDeserializeObjectValue()
	  {
		// given
		ObjectValue value = Variables.objectValue("value").create();
		VariableMap variables = Variables.createVariables().putValue("var", value);

		runtimeService.startProcessInstanceByKey("twoTasksProcess", variables);

		Task task = taskService.createTaskQuery().singleResult();

		// when
		VariableMap result = formService.submitTaskFormWithVariablesInReturn(task.Id, null, true);

		// then
		ObjectValue returnedValue = result.getValueTyped("var");
		assertThat(returnedValue.Deserialized).True;
		assertThat(returnedValue.Value).isEqualTo("value");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml") public void testSubmitTaskFormWithVarialbesInReturnShouldNotDeserializeObjectValue()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml")]
	  public virtual void testSubmitTaskFormWithVarialbesInReturnShouldNotDeserializeObjectValue()
	  {
		// given
		ObjectValue value = Variables.objectValue("value").create();
		VariableMap variables = Variables.createVariables().putValue("var", value);

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("twoTasksProcess", variables);
		string serializedValue = ((ObjectValue) runtimeService.getVariableTyped(instance.Id, "var")).ValueSerialized;

		Task task = taskService.createTaskQuery().singleResult();

		// when
		VariableMap result = formService.submitTaskFormWithVariablesInReturn(task.Id, null, false);

		// then
		ObjectValue returnedValue = result.getValueTyped("var");
		assertThat(returnedValue.Deserialized).False;
		assertThat(returnedValue.ValueSerialized).isEqualTo(serializedValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testSubmitTaskFormContainingReadonlyVariable()
	  public virtual void testSubmitTaskFormContainingReadonlyVariable()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(processDefinition.Id);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		formService.submitTaskForm(task.Id, new Dictionary<string, object>());

		testRule.assertProcessEnded(processInstance.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testGetTaskFormWithoutLabels()
	  public virtual void testGetTaskFormWithoutLabels()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		Task task = taskService.createTaskQuery().singleResult();

		// form data can be retrieved
		TaskFormData formData = formService.getTaskFormData(task.Id);

		IList<FormField> formFields = formData.FormFields;
		assertEquals(3, formFields.Count);

		IList<string> formFieldIds = new List<string>();
		foreach (FormField field in formFields)
		{
		  assertNull(field.Label);
		  formFieldIds.Add(field.Id);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		assertTrue(formFieldIds.containsAll(Arrays.asList("stringField", "customField", "longField")));

		// the form can be rendered
		object startForm = formService.getRenderedTaskForm(task.Id);
		assertNotNull(startForm);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployTaskFormWithoutFieldTypes()
	  public virtual void testDeployTaskFormWithoutFieldTypes()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/form/FormServiceTest.testDeployTaskFormWithoutFieldTypes.bpmn20.xml").deploy();
		}
		catch (ProcessEngineException e)
		{
		  testRule.assertTextPresent("form field must have a 'type' attribute", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testGetStartFormWithoutLabels()
	  public virtual void testGetStartFormWithoutLabels()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceById(processDefinition.Id);

		// form data can be retrieved
		StartFormData formData = formService.getStartFormData(processDefinition.Id);

		IList<FormField> formFields = formData.FormFields;
		assertEquals(3, formFields.Count);

		IList<string> formFieldIds = new List<string>();
		foreach (FormField field in formFields)
		{
		  assertNull(field.Label);
		  formFieldIds.Add(field.Id);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		assertTrue(formFieldIds.containsAll(Arrays.asList("stringField", "customField", "longField")));

		// the form can be rendered
		object startForm = formService.getRenderedStartForm(processDefinition.Id);
		assertNotNull(startForm);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployStartFormWithoutFieldTypes()
	  public virtual void testDeployStartFormWithoutFieldTypes()
	  {
		try
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/form/FormServiceTest.testDeployStartFormWithoutFieldTypes.bpmn20.xml").deploy();
		}
		catch (ProcessEngineException e)
		{
		  testRule.assertTextPresent("form field must have a 'type' attribute", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/util/approve.form", "org/camunda/bpm/engine/test/api/form/util/request.form", "org/camunda/bpm/engine/test/api/form/util/adjustRequest.form" }) @Test public void testTaskFormsWithVacationRequestProcess()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/util/approve.form", "org/camunda/bpm/engine/test/api/form/util/request.form", "org/camunda/bpm/engine/test/api/form/util/adjustRequest.form" })]
	  public virtual void testTaskFormsWithVacationRequestProcess()
	  {

		// Get start form
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;
		object startForm = formService.getRenderedStartForm(procDefId, "juel");
		assertNotNull(startForm);

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		string processDefinitionId = processDefinition.Id;
		assertEquals("org/camunda/bpm/engine/test/api/form/util/request.form", formService.getStartFormData(processDefinitionId).FormKey);

		// Define variables that would be filled in through the form
		IDictionary<string, string> formProperties = new Dictionary<string, string>();
		formProperties["employeeName"] = "kermit";
		formProperties["numberOfDays"] = "4";
		formProperties["vacationMotivation"] = "I'm tired";
		formService.submitStartFormData(procDefId, formProperties);

		// Management should now have a task assigned to them
		Task task = taskService.createTaskQuery().taskCandidateGroup("management").singleResult();
		assertEquals("Vacation request by kermit", task.Description);
		object taskForm = formService.getRenderedTaskForm(task.Id, "juel");
		assertNotNull(taskForm);

		// Rejecting the task should put the process back to first task
		taskService.complete(task.Id, CollectionUtil.singletonMap("vacationApproved", "false"));
		task = taskService.createTaskQuery().singleResult();
		assertEquals("Adjust vacation request", task.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testTaskFormUnavailable()
	  public virtual void testTaskFormUnavailable()
	  {
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;
		assertNull(formService.getRenderedStartForm(procDefId));

		runtimeService.startProcessInstanceByKey("noStartOrTaskForm");
		Task task = taskService.createTaskQuery().singleResult();
		assertNull(formService.getRenderedTaskForm(task.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testBusinessKey()
	  public virtual void testBusinessKey()
	  {
		// given
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		// when
		StartFormData startFormData = formService.getStartFormData(procDefId);

		// then
		FormField formField = startFormData.FormFields[0];
		assertTrue(formField.BusinessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testSubmitStartFormWithFormFieldMarkedAsBusinessKey()
	  public virtual void testSubmitStartFormWithFormFieldMarkedAsBusinessKey()
	  {
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;
		ProcessInstance pi = formService.submitStartForm(procDefId, "foo", Variables.createVariables().putValue("secondParam", "bar"));

		assertEquals("foo", pi.BusinessKey);

		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();
		assertEquals(1, result.Count);
		assertTrue(result[0].Name.Equals("secondParam"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/form/DeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form", "org/camunda/bpm/engine/test/api/form/task.form" }) @Test public void testGetDeployedStartForm()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/form/DeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form", "org/camunda/bpm/engine/test/api/form/task.form" })]
	  public virtual void testGetDeployedStartForm()
	  {
		// given
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		// when
		Stream deployedStartForm = formService.getDeployedStartForm(procDefId);

		// then
		assertNotNull(deployedStartForm);
		string fileAsString = IoUtil.fileAsString("org/camunda/bpm/engine/test/api/form/start.form");
		string deployedStartFormAsString = IoUtil.inputStreamAsString(deployedStartForm);
		assertEquals(deployedStartFormAsString, fileAsString);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/form/EmbeddedDeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form", "org/camunda/bpm/engine/test/api/form/task.form" }) @Test public void testGetEmbeddedDeployedStartForm()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/form/EmbeddedDeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form", "org/camunda/bpm/engine/test/api/form/task.form" })]
	  public virtual void testGetEmbeddedDeployedStartForm()
	  {
		// given
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		// when
		Stream deployedStartForm = formService.getDeployedStartForm(procDefId);

		// then
		assertNotNull(deployedStartForm);
		string fileAsString = IoUtil.fileAsString("org/camunda/bpm/engine/test/api/form/start.form");
		string deployedStartFormAsString = IoUtil.inputStreamAsString(deployedStartForm);
		assertEquals(deployedStartFormAsString, fileAsString);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedStartFormWithNullProcDefId()
	  public virtual void testGetDeployedStartFormWithNullProcDefId()
	  {
		try
		{
		  formService.getDeployedStartForm(null);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertEquals("Process definition id cannot be null: processDefinitionId is null", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/form/DeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form", "org/camunda/bpm/engine/test/api/form/task.form" }) @Test public void testGetDeployedTaskForm()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/form/DeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form", "org/camunda/bpm/engine/test/api/form/task.form" })]
	  public virtual void testGetDeployedTaskForm()
	  {
		// given
		runtimeService.startProcessInstanceByKey("FormsProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		Stream deployedTaskForm = formService.getDeployedTaskForm(taskId);

		// then
		assertNotNull(deployedTaskForm);
		string fileAsString = IoUtil.fileAsString("org/camunda/bpm/engine/test/api/form/task.form");
		string deployedStartFormAsString = IoUtil.inputStreamAsString(deployedTaskForm);
		assertEquals(deployedStartFormAsString, fileAsString);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/form/DeployedFormsCase.cmmn11.xml", "org/camunda/bpm/engine/test/api/form/task.form" }) @Test public void testGetDeployedTaskForm_Case()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/form/DeployedFormsCase.cmmn11.xml", "org/camunda/bpm/engine/test/api/form/task.form" })]
	  public virtual void testGetDeployedTaskForm_Case()
	  {
		// given
		caseService.createCaseInstanceByKey("Case_1");
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		Stream deployedTaskForm = formService.getDeployedTaskForm(taskId);

		// then
		assertNotNull(deployedTaskForm);
		string fileAsString = IoUtil.fileAsString("org/camunda/bpm/engine/test/api/form/task.form");
		string deployedStartFormAsString = IoUtil.inputStreamAsString(deployedTaskForm);
		assertEquals(deployedStartFormAsString, fileAsString);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/form/EmbeddedDeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form", "org/camunda/bpm/engine/test/api/form/task.form" }) @Test public void testGetEmbeddedDeployedTaskForm()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/form/EmbeddedDeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form", "org/camunda/bpm/engine/test/api/form/task.form" })]
	  public virtual void testGetEmbeddedDeployedTaskForm()
	  {
		// given
		runtimeService.startProcessInstanceByKey("FormsProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		Stream deployedTaskForm = formService.getDeployedTaskForm(taskId);

		// then
		assertNotNull(deployedTaskForm);
		string fileAsString = IoUtil.fileAsString("org/camunda/bpm/engine/test/api/form/task.form");
		string deployedStartFormAsString = IoUtil.inputStreamAsString(deployedTaskForm);
		assertEquals(deployedStartFormAsString, fileAsString);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedTaskFormWithNullTaskId()
	  public virtual void testGetDeployedTaskFormWithNullTaskId()
	  {
		try
		{
		  formService.getDeployedTaskForm(null);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  assertEquals("Task id cannot be null: taskId is null", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/form/DeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/task.form" }) @Test public void testGetDeployedStartForm_DeploymentNotFound()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/form/DeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/task.form" })]
	  public virtual void testGetDeployedStartForm_DeploymentNotFound()
	  {
		// given
		string procDefId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		try
		{
		  // when
		  formService.getDeployedStartForm(procDefId);
		  fail("Exception expected");
		}
		catch (NotFoundException e)
		{
		  // then
		  testRule.assertTextPresent("The form with the resource name 'org/camunda/bpm/engine/test/api/form/start.form' cannot be found in deployment.", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/form/DeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form" }) @Test public void testGetDeployedTaskForm_DeploymentNotFound()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/form/DeployedFormsProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/form/start.form" })]
	  public virtual void testGetDeployedTaskForm_DeploymentNotFound()
	  {
		// given
		runtimeService.startProcessInstanceByKey("FormsProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;

		try
		{
		  // when
		  formService.getDeployedTaskForm(taskId);
		  fail("Exception expected");
		}
		catch (NotFoundException e)
		{
		  // then
		  testRule.assertTextPresent("The form with the resource name 'org/camunda/bpm/engine/test/api/form/task.form' cannot be found in deployment.", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedStartForm_FormKeyNotSet()
	  public virtual void testGetDeployedStartForm_FormKeyNotSet()
	  {
		// given
		testRule.deploy(ProcessModels.ONE_TASK_PROCESS);
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		// when
		try
		{
		  formService.getDeployedStartForm(processDefinitionId);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  testRule.assertTextPresent("The form key is not set.", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDeployedTaskForm_FormKeyNotSet()
	  public virtual void testGetDeployedTaskForm_FormKeyNotSet()
	  {
		// given
		testRule.deploy(ProcessModels.ONE_TASK_PROCESS);
		runtimeService.startProcessInstanceByKey("Process");
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		try
		{
		  formService.getDeployedTaskForm(taskId);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  // then
		  testRule.assertTextPresent("The form key is not set.", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/form/FormServiceTest.testGetDeployedStartFormWithWrongKeyFormat.bpmn20.xml" }) @Test public void testGetDeployedStartFormWithWrongKeyFormat()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/form/FormServiceTest.testGetDeployedStartFormWithWrongKeyFormat.bpmn20.xml" })]
	  public virtual void testGetDeployedStartFormWithWrongKeyFormat()
	  {
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().singleResult().Id;

		try
		{
		  formService.getDeployedStartForm(processDefinitionId);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  testRule.assertTextPresent("The form key 'formKey' does not reference a deployed form.", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/form/FormServiceTest.testGetDeployedTaskFormWithWrongKeyFormat.bpmn20.xml" }) @Test public void testGetDeployedTaskFormWithWrongKeyFormat()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/form/FormServiceTest.testGetDeployedTaskFormWithWrongKeyFormat.bpmn20.xml" })]
	  public virtual void testGetDeployedTaskFormWithWrongKeyFormat()
	  {
		runtimeService.startProcessInstanceByKey("FormsProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;

		try
		{
		  formService.getDeployedTaskForm(taskId);
		  fail("Exception expected");
		}
		catch (BadUserRequestException e)
		{
		  testRule.assertTextPresent("The form key 'formKey' does not reference a deployed form.", e.Message);
		}
	  }
	}

}