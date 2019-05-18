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
namespace org.camunda.bpm.model.xml.validation
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.*;


	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using TestModelParser = org.camunda.bpm.model.xml.testmodel.TestModelParser;
	using Bird = org.camunda.bpm.model.xml.testmodel.instance.Bird;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ModelValidationTest
	{

	  protected internal ModelInstance modelInstance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void parseModel()
	  public virtual void parseModel()
	  {
		TestModelParser modelParser = new TestModelParser();
		string testXml = "org/camunda/bpm/model/xml/testmodel/instance/UnknownAnimalTest.xml";
		Stream testXmlAsStream = this.GetType().ClassLoader.getResourceAsStream(testXml);

		modelInstance = modelParser.parseModelFromStream(testXmlAsStream);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldValidateWithEmptyList()
	  public virtual void shouldValidateWithEmptyList()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<ModelElementValidator<?>> validators = new java.util.ArrayList<ModelElementValidator<?>>();
		IList<ModelElementValidator<object>> validators = new List<ModelElementValidator<object>>();

		ValidationResults results = modelInstance.validate(validators);

		assertThat(results).NotNull;
		assertThat(results.hasErrors()).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCollectWarnings()
	  public virtual void shouldCollectWarnings()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<ModelElementValidator<?>> validators = new java.util.ArrayList<ModelElementValidator<?>>();
		IList<ModelElementValidator<object>> validators = new List<ModelElementValidator<object>>();

		validators.Add(new IsAdultWarner());

		ValidationResults results = modelInstance.validate(validators);

		assertThat(results).NotNull;
		assertThat(results.hasErrors()).False;
		assertThat(results.ErrorCount).isEqualTo(0);
		assertThat(results.WarinigCount).isEqualTo(7);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCollectErrors()
	  public virtual void shouldCollectErrors()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<ModelElementValidator<?>> validators = new java.util.ArrayList<ModelElementValidator<?>>();
		IList<ModelElementValidator<object>> validators = new List<ModelElementValidator<object>>();

		validators.Add(new IllegalBirdValidator("tweety"));

		ValidationResults results = modelInstance.validate(validators);

		assertThat(results).NotNull;
		assertThat(results.hasErrors()).True;
		assertThat(results.ErrorCount).isEqualTo(1);
		assertThat(results.WarinigCount).isEqualTo(0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteResults()
	  public virtual void shouldWriteResults()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<ModelElementValidator<?>> validators = new java.util.ArrayList<ModelElementValidator<?>>();
		IList<ModelElementValidator<object>> validators = new List<ModelElementValidator<object>>();

		validators.Add(new IllegalBirdValidator("tweety"));

		ValidationResults results = modelInstance.validate(validators);

		StringWriter stringWriter = new StringWriter();
		results.write(stringWriter, new TestResultFormatter());

		assertThat(stringWriter.ToString()).isEqualTo("tweety\n\tERROR (20): Bird tweety is illegal\n");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnResults()
	  public virtual void shouldReturnResults()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<ModelElementValidator<?>> validators = new java.util.ArrayList<ModelElementValidator<?>>();
		IList<ModelElementValidator<object>> validators = new List<ModelElementValidator<object>>();

		validators.Add(new IllegalBirdValidator("tweety"));
		validators.Add(new IsAdultWarner());

		ValidationResults results = modelInstance.validate(validators);

		assertThat(results.ErrorCount).isEqualTo(1);
		assertThat(results.WarinigCount).isEqualTo(7);

		IDictionary<ModelElementInstance, IList<ValidationResult>> resultsByElement = results.Results;
		assertThat(resultsByElement.Count).isEqualTo(7);

		foreach (KeyValuePair<ModelElementInstance, IList<ValidationResult>> resultEntry in resultsByElement.SetOfKeyValuePairs())
		{
		  Bird element = (Bird) resultEntry.Key;
		  IList<ValidationResult> validationResults = resultEntry.Value;
		  assertThat(element).NotNull;
		  assertThat(validationResults).NotNull;

		  if (element.Id.Equals("tweety"))
		  {
			assertThat(validationResults.Count).isEqualTo(2);
			ValidationResult error = validationResults.RemoveAt(0);
			assertThat(error.Type).isEqualTo(ValidationResultType.ERROR);
			assertThat(error.Code).isEqualTo(20);
			assertThat(error.Message).isEqualTo("Bird tweety is illegal");
			assertThat(error.Element).isEqualTo(element);
		  }
		  else
		  {
			assertThat(validationResults.Count).isEqualTo(1);
		  }

		  ValidationResult warning = validationResults[0];
		  assertThat(warning.Type).isEqualTo(ValidationResultType.WARNING);
		  assertThat(warning.Code).isEqualTo(10);
		  assertThat(warning.Message).isEqualTo("Is not an adult");
		  assertThat(warning.Element).isEqualTo(element);
		}
	  }
	}

}