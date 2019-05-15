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
namespace org.camunda.bpm.engine.impl.form.validator
{
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class AbstractNumericValidator : FormFieldValidator
	{

	  public virtual bool validate(object submittedValue, FormFieldValidatorContext validatorContext)
	  {

		if (submittedValue == null)
		{
		  return NullValid;
		}

		string configurationString = validatorContext.Configuration;

		// Double

		if (submittedValue is double?)
		{
		  double? configuration = null;
		  try
		  {
			configuration = double.Parse(configurationString);
		  }
		  catch (System.FormatException)
		  {
			throw new FormFieldConfigurationException(configurationString, "Cannot validate Double value " + submittedValue + ": configuration " + configurationString + " cannot be parsed as Double.");
		  }
		  return validate((double?) submittedValue, configuration);
		}

		// Float

		if (submittedValue is float?)
		{
		  float? configuration = null;
		  try
		  {
			configuration = float.Parse(configurationString);
		  }
		  catch (System.FormatException)
		  {
			throw new FormFieldConfigurationException(configurationString, "Cannot validate Float value " + submittedValue + ": configuration " + configurationString + " cannot be parsed as Float.");
		  }
		  return validate((float?) submittedValue, configuration);
		}

		// Long

		if (submittedValue is long?)
		{
		  long? configuration = null;
		  try
		  {
			configuration = long.Parse(configurationString);
		  }
		  catch (System.FormatException)
		  {
			throw new FormFieldConfigurationException(configurationString, "Cannot validate Long value " + submittedValue + ": configuration " + configurationString + " cannot be parsed as Long.");
		  }
		  return validate((long?) submittedValue, configuration);
		}

		// Integer

		if (submittedValue is int?)
		{
		  int? configuration = null;
		  try
		  {
			configuration = int.Parse(configurationString);
		  }
		  catch (System.FormatException)
		  {
			throw new FormFieldConfigurationException(configurationString, "Cannot validate Integer value " + submittedValue + ": configuration " + configurationString + " cannot be parsed as Integer.");
		  }
		  return validate((int?) submittedValue, configuration);
		}

		// Short

		if (submittedValue is short?)
		{
		  short? configuration = null;
		  try
		  {
			configuration = short.Parse(configurationString);
		  }
		  catch (System.FormatException)
		  {
			throw new FormFieldConfigurationException(configurationString, "Cannot validate Short value " + submittedValue + ": configuration " + configurationString + " cannot be parsed as Short.");
		  }
		  return validate((short?) submittedValue, configuration);
		}

		throw new FormFieldValidationException("Numeric validator " + this.GetType().Name + " cannot be used on non-numeric value " + submittedValue);
	  }

	  protected internal virtual bool NullValid
	  {
		  get
		  {
			return true;
		  }
	  }

	  protected internal abstract bool validate(int? submittedValue, int? configuration);

	  protected internal abstract bool validate(long? submittedValue, long? configuration);

	  protected internal abstract bool validate(double? submittedValue, double? configuration);

	  protected internal abstract bool validate(float? submittedValue, float? configuration);

	  protected internal abstract bool validate(short? submittedValue, short? configuration);

	}

}