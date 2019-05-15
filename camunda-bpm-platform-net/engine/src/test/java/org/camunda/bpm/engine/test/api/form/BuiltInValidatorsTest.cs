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

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using FormException = org.camunda.bpm.engine.impl.form.FormException;
	using FormFieldHandler = org.camunda.bpm.engine.impl.form.handler.FormFieldHandler;
	using FormFieldValidator = org.camunda.bpm.engine.impl.form.validator.FormFieldValidator;
	using FormFieldValidatorContext = org.camunda.bpm.engine.impl.form.validator.FormFieldValidatorContext;
	using FormValidators = org.camunda.bpm.engine.impl.form.validator.FormValidators;
	using MaxLengthValidator = org.camunda.bpm.engine.impl.form.validator.MaxLengthValidator;
	using MaxValidator = org.camunda.bpm.engine.impl.form.validator.MaxValidator;
	using MinLengthValidator = org.camunda.bpm.engine.impl.form.validator.MinLengthValidator;
	using MinValidator = org.camunda.bpm.engine.impl.form.validator.MinValidator;
	using ReadOnlyValidator = org.camunda.bpm.engine.impl.form.validator.ReadOnlyValidator;
	using RequiredValidator = org.camunda.bpm.engine.impl.form.validator.RequiredValidator;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using TestVariableScope = org.camunda.bpm.engine.test.api.runtime.util.TestVariableScope;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class BuiltInValidatorsTest : PluggableProcessEngineTestCase
	{

	  public virtual void testDefaultFormFieldValidators()
	  {

		// assert default validators are registered
		FormValidators formValidators = ((ProcessEngineImpl) processEngine).ProcessEngineConfiguration.FormValidators;

		IDictionary<string, Type> validators = formValidators.Validators;
		assertEquals(typeof(RequiredValidator), validators["required"]);
		assertEquals(typeof(ReadOnlyValidator), validators["readonly"]);
		assertEquals(typeof(MinValidator), validators["min"]);
		assertEquals(typeof(MaxValidator), validators["max"]);
		assertEquals(typeof(MaxLengthValidator), validators["maxlength"]);
		assertEquals(typeof(MinLengthValidator), validators["minlength"]);

	  }

	  public virtual void testRequiredValidator()
	  {
		RequiredValidator validator = new RequiredValidator();
		TestValidatorContext validatorContext = new TestValidatorContext(null);

		assertTrue(validator.validate("test", validatorContext));
		assertTrue(validator.validate(1, validatorContext));
		assertTrue(validator.validate(true, validatorContext));

		// empty string and 'null' are invalid
		assertFalse(validator.validate("", validatorContext));
		assertFalse(validator.validate(null, validatorContext));

		// can submit null if the value already exists
		validatorContext = new TestValidatorContext(null, "fieldName");
		validatorContext.VariableScope.setVariable("fieldName", "existingValue");
		assertTrue(validator.validate(null, validatorContext));
	  }

	  public virtual void testReadOnlyValidator()
	  {
		ReadOnlyValidator validator = new ReadOnlyValidator();

		assertFalse(validator.validate("", null));
		assertFalse(validator.validate("aaa", null));
		assertFalse(validator.validate(11, null));
		assertFalse(validator.validate(2d, null));
		assertTrue(validator.validate(null, null));
	  }

	  public virtual void testMinValidator()
	  {
		MinValidator validator = new MinValidator();

		assertTrue(validator.validate(null, null));

		assertTrue(validator.validate(4, new TestValidatorContext("4")));
		assertFalse(validator.validate(4, new TestValidatorContext("5")));

		try
		{
		  validator.validate(4, new TestValidatorContext("4.4"));
		  fail("exception expected");
		}
		catch (FormException e)
		{
		  assertTrue(e.Message.contains("Cannot validate Integer value 4: configuration 4.4 cannot be parsed as Integer."));
		}

		assertFalse(validator.validate(4d, new TestValidatorContext("4.1")));
		assertTrue(validator.validate(4.1d, new TestValidatorContext("4.1")));

		assertFalse(validator.validate(4f, new TestValidatorContext("4.1")));
		assertTrue(validator.validate(4.1f, new TestValidatorContext("4.1")));

	  }

	  public virtual void testMaxValidator()
	  {
		MaxValidator validator = new MaxValidator();

		assertTrue(validator.validate(null, null));

		assertTrue(validator.validate(3, new TestValidatorContext("4")));
		assertFalse(validator.validate(4, new TestValidatorContext("3")));

		try
		{
		  validator.validate(4, new TestValidatorContext("4.4"));
		  fail("exception expected");
		}
		catch (FormException e)
		{
		  assertTrue(e.Message.contains("Cannot validate Integer value 4: configuration 4.4 cannot be parsed as Integer."));
		}

		assertFalse(validator.validate(4.1d, new TestValidatorContext("4")));
		assertTrue(validator.validate(4.1d, new TestValidatorContext("4.2")));

		assertFalse(validator.validate(4.1f, new TestValidatorContext("4")));
		assertTrue(validator.validate(4.1f, new TestValidatorContext("4.2")));

	  }

	  public virtual void testMaxLengthValidator()
	  {
		MaxLengthValidator validator = new MaxLengthValidator();

		assertTrue(validator.validate(null, null));

		assertTrue(validator.validate("test", new TestValidatorContext("5")));
		assertFalse(validator.validate("test", new TestValidatorContext("4")));

		try
		{
		  validator.validate("test", new TestValidatorContext("4.4"));
		  fail("exception expected");
		}
		catch (FormException e)
		{
		  assertTrue(e.Message.contains("Cannot validate \"maxlength\": configuration 4.4 cannot be interpreted as Integer"));
		}
	  }

	  public virtual void testMinLengthValidator()
	  {
		MinLengthValidator validator = new MinLengthValidator();

		assertTrue(validator.validate(null, null));

		assertTrue(validator.validate("test", new TestValidatorContext("4")));
		assertFalse(validator.validate("test", new TestValidatorContext("5")));

		try
		{
		  validator.validate("test", new TestValidatorContext("4.4"));
		  fail("exception expected");
		}
		catch (FormException e)
		{
		  assertTrue(e.Message.contains("Cannot validate \"minlength\": configuration 4.4 cannot be interpreted as Integer"));
		}
	  }

	  protected internal class TestValidatorContext : FormFieldValidatorContext
	  {

		internal TestVariableScope variableScope = new TestVariableScope();
		internal FormFieldHandler formFieldHandler = new FormFieldHandler();
		internal string configuration;

		public TestValidatorContext(string configuration)
		{
		  this.configuration = configuration;
		}

		public TestValidatorContext(string configuration, string formFieldId)
		{
		  this.configuration = configuration;
		  this.formFieldHandler.Id = formFieldId;
		}

		public virtual FormFieldHandler FormFieldHandler
		{
			get
			{
			  return formFieldHandler;
			}
		}

		public virtual DelegateExecution Execution
		{
			get
			{
			  return null;
			}
		}

		public virtual string Configuration
		{
			get
			{
			  return configuration;
			}
		}

		public virtual IDictionary<string, object> SubmittedValues
		{
			get
			{
			  return null;
			}
		}

		public virtual VariableScope VariableScope
		{
			get
			{
			  return variableScope;
			}
		}
	  }

	}

}