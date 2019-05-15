using System;

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
	using FormFieldValidationException = org.camunda.bpm.engine.impl.form.validator.FormFieldValidationException;
	using FormFieldValidator = org.camunda.bpm.engine.impl.form.validator.FormFieldValidator;
	using FormFieldValidatorContext = org.camunda.bpm.engine.impl.form.validator.FormFieldValidatorContext;

	/// <summary>
	/// @author Thomas Skjolberg
	/// 
	/// </summary>
	public class CustomValidatorWithDetail : FormFieldValidator
	{

	  public CustomValidatorWithDetail()
	  {
		Console.WriteLine("CREATED");
	  }

	  public virtual bool validate(object submittedValue, FormFieldValidatorContext validatorContext)
	  {
		if (submittedValue == null)
		{
		  return true;
		}

		if (submittedValue.Equals("A") || submittedValue.Equals("B"))
		{
		  return true;
		}

		if (submittedValue.Equals("C"))
		{
		  // instead of returning false, use an exception to specify details about
		  // what went wrong
		  throw new FormFieldValidationException("EXPIRED", "Unable to validate " + submittedValue);
		}

		// return false in the generic case
		return false;
	  }

	}

}