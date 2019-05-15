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
namespace org.camunda.bpm.engine.test.api.form
{

	using FormField = org.camunda.bpm.engine.form.FormField;
	using FormFieldValidationConstraint = org.camunda.bpm.engine.form.FormFieldValidationConstraint;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using EnumFormType = org.camunda.bpm.engine.impl.form.type.EnumFormType;
	using FormFieldValidationException = org.camunda.bpm.engine.impl.form.validator.FormFieldValidationException;
	using FormFieldValidatorException = org.camunda.bpm.engine.impl.form.validator.FormFieldValidatorException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// <para>Testcase verifying support for form matadata provided using
	/// custom extension elements in BPMN Xml</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class FormDataTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testGetFormFieldBasicProperties()
	  public virtual void testGetFormFieldBasicProperties()
	  {

		runtimeService.startProcessInstanceByKey("FormDataTest.testGetFormFieldBasicProperties");

		Task task = taskService.createTaskQuery().singleResult();
		TaskFormData taskFormData = formService.getTaskFormData(task.Id);

		// validate properties:
		IList<FormField> formFields = taskFormData.FormFields;

		// validate field 1
		FormField formField1 = formFields[0];
		assertNotNull(formField1);
		assertEquals(formField1.Id, "formField1");
		assertEquals(formField1.Label, "Form Field 1");
		assertEquals("string", formField1.TypeName);
		assertNotNull(formField1.Type);

		// validate field 2
		FormField formField2 = formFields[1];
		assertNotNull(formField2);
		assertEquals(formField2.Id, "formField2");
		assertEquals(formField2.Label, "Form Field 2");
		assertEquals("boolean", formField2.TypeName);
		assertNotNull(formField1.Type);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testGetFormFieldBuiltInTypes()
	  public virtual void testGetFormFieldBuiltInTypes()
	  {

		runtimeService.startProcessInstanceByKey("FormDataTest.testGetFormFieldBuiltInTypes");

		Task task = taskService.createTaskQuery().singleResult();

		TaskFormData taskFormData = formService.getTaskFormData(task.Id);

		// validate properties:
		IList<FormField> formFields = taskFormData.FormFields;

		// validate string field
		FormField stringField = formFields[0];
		assertNotNull(stringField);
		assertEquals("string", stringField.TypeName);
		assertNotNull(stringField.Type);
		assertEquals("someString", stringField.DefaultValue);

		// validate long field
		FormField longField = formFields[1];
		assertNotNull(longField);
		assertEquals("long", longField.TypeName);
		assertNotNull(longField.Type);
		assertEquals(Convert.ToInt64(1l), longField.DefaultValue);

		// validate boolean field
		FormField booleanField = formFields[2];
		assertNotNull(booleanField);
		assertEquals("boolean", booleanField.TypeName);
		assertNotNull(booleanField.Type);
		assertEquals(Convert.ToBoolean(true), booleanField.DefaultValue);

		// validate date field
		FormField dateField = formFields[3];
		assertNotNull(dateField);
		assertEquals("date", dateField.TypeName);
		assertNotNull(dateField.Type);
		DateTime dateValue = (DateTime) dateField.DefaultValue;
		DateTime calendar = new DateTime();
		calendar = new DateTime(dateValue);
		assertEquals(10, calendar.Day);
		assertEquals(1, calendar.Month);
		assertEquals(2013, calendar.Year);

		// validate enum field
		FormField enumField = formFields[4];
		assertNotNull(enumField);
		assertEquals("enum", enumField.TypeName);
		assertNotNull(enumField.Type);
		EnumFormType enumFormType = (EnumFormType) enumField.Type;
		IDictionary<string, string> values = enumFormType.Values;
		assertEquals("A", values["a"]);
		assertEquals("B", values["b"]);
		assertEquals("C", values["c"]);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testGetFormFieldProperties()
	  public virtual void testGetFormFieldProperties()
	  {

		runtimeService.startProcessInstanceByKey("FormDataTest.testGetFormFieldProperties");

		Task task = taskService.createTaskQuery().singleResult();

		TaskFormData taskFormData = formService.getTaskFormData(task.Id);

		IList<FormField> formFields = taskFormData.FormFields;

		FormField stringField = formFields[0];
		IDictionary<string, string> properties = stringField.Properties;
		assertEquals("property1", properties["p1"]);
		assertEquals("property2", properties["p2"]);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testGetFormFieldValidationConstraints()
	  public virtual void testGetFormFieldValidationConstraints()
	  {

		runtimeService.startProcessInstanceByKey("FormDataTest.testGetFormFieldValidationConstraints");

		Task task = taskService.createTaskQuery().singleResult();

		TaskFormData taskFormData = formService.getTaskFormData(task.Id);

		IList<FormField> formFields = taskFormData.FormFields;

		FormField field1 = formFields[0];
		IList<FormFieldValidationConstraint> validationConstraints = field1.ValidationConstraints;
		FormFieldValidationConstraint constraint1 = validationConstraints[0];
		assertEquals("maxlength", constraint1.Name);
		assertEquals("10", constraint1.Configuration);
		FormFieldValidationConstraint constraint2 = validationConstraints[1];
		assertEquals("minlength", constraint2.Name);
		assertEquals("5", constraint2.Configuration);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testFormFieldSubmit()
	  public virtual void testFormFieldSubmit()
	  {

		// valid submit
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("FormDataTest.testFormFieldSubmit");
		Task task = taskService.createTaskQuery().singleResult();
		IDictionary<string, object> formValues = new Dictionary<string, object>();
		formValues["stringField"] = "12345";
		formValues["longField"] = 9L;
		formValues["customField"] = "validValue";
		formService.submitTaskForm(task.Id, formValues);

		assertEquals(formValues, runtimeService.getVariables(processInstance.Id));
		runtimeService.deleteProcessInstance(processInstance.Id, "test complete");

		runtimeService.startProcessInstanceByKey("FormDataTest.testFormFieldSubmit");
		task = taskService.createTaskQuery().singleResult();
		// invalid submit 1

		formValues = new Dictionary<string, object>();
		formValues["stringField"] = "1234";
		formValues["longField"] = 9L;
		formValues["customField"] = "validValue";
		try
		{
		  formService.submitTaskForm(task.Id, formValues);
		  fail();
		}
		catch (FormFieldValidatorException e)
		{
		  assertEquals(e.Name, "minlength");
		}

		// invalid submit 2
		formValues = new Dictionary<string, object>();

		formValues["customFieldWithValidationDetails"] = "C";
		try
		{
		  formService.submitTaskForm(task.Id, formValues);
		  fail();
		}
		catch (FormFieldValidatorException e)
		{
		  assertEquals(e.Name, "validator");
		  assertEquals(e.Id, "customFieldWithValidationDetails");

		  assertTrue(e.InnerException is FormFieldValidationException);

		  FormFieldValidationException exception = (FormFieldValidationException) e.InnerException;
		  assertEquals(exception.Detail, "EXPIRED");
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSubmitFormDataWithEmptyDate()
	  public virtual void testSubmitFormDataWithEmptyDate()
	  {
		// given
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("FormDataTest.testSubmitFormDataWithEmptyDate");
		Task task = taskService.createTaskQuery().singleResult();
		IDictionary<string, object> formValues = new Dictionary<string, object>();
		formValues["stringField"] = "12345";
		formValues["dateField"] = "";

		// when
		formService.submitTaskForm(task.Id, formValues);

		// then
		formValues["dateField"] = null;
		assertEquals(formValues, runtimeService.getVariables(processInstance.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMissingFormVariables()
	  public virtual void testMissingFormVariables()
	  {
		// given process definition with defined form varaibles
		// when start process instance with no variables
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("date-form-property-test");
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		// then taskFormData contains form variables with null as values
		TaskFormData taskFormData = formService.getTaskFormData(task.Id);
		assertNotNull(taskFormData);
		assertEquals(5, taskFormData.FormFields.Count);
		foreach (FormField field in taskFormData.FormFields)
		{
		  assertNotNull(field);
		  assertNull(field.Value.Value);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/form/FormDataTest.testDoubleQuotesAreEscapedInGeneratedTaskForms.bpmn20.xml")]
	  public virtual void testDoubleQuotesAreEscapedInGeneratedTaskForms()
	  {

		// given
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables["foo"] = "This is a \"Test\" message!";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);
		Task taskWithForm = taskService.createTaskQuery().singleResult();

		// when
		object renderedStartForm = formService.getRenderedTaskForm(taskWithForm.Id);
		assertTrue(renderedStartForm is string);

		// then
		string renderedForm = (string) renderedStartForm;
		string expectedFormValueWithEscapedQuotes = "This is a &quot;Test&quot; message!";
		assertTrue(renderedForm.Contains(expectedFormValueWithEscapedQuotes));

	  }

	}

}