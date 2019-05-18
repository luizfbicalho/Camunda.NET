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
namespace org.camunda.bpm.model.xml.impl.validation
{

	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ValidationResultFormatter = org.camunda.bpm.model.xml.validation.ValidationResultFormatter;
	using ValidationResult = org.camunda.bpm.model.xml.validation.ValidationResult;
	using ValidationResults = org.camunda.bpm.model.xml.validation.ValidationResults;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ModelValidationResultsImpl : ValidationResults
	{

	  protected internal IDictionary<ModelElementInstance, IList<ValidationResult>> collectedResults;

	  protected internal int errorCount;
	  protected internal int warningCount;

	  public ModelValidationResultsImpl(IDictionary<ModelElementInstance, IList<ValidationResult>> collectedResults, int errorCount, int warningCount)
	  {
		this.collectedResults = collectedResults;
		this.errorCount = errorCount;
		this.warningCount = warningCount;
	  }

	  public virtual bool hasErrors()
	  {
		return errorCount > 0;
	  }

	  public virtual int ErrorCount
	  {
		  get
		  {
			return errorCount;
		  }
	  }

	  public virtual int WarinigCount
	  {
		  get
		  {
			return warningCount;
		  }
	  }

	  public virtual void write(StringWriter writer, ValidationResultFormatter formatter)
	  {
		foreach (KeyValuePair<ModelElementInstance, IList<ValidationResult>> entry in collectedResults.SetOfKeyValuePairs())
		{

		  ModelElementInstance element = entry.Key;
		  IList<ValidationResult> results = entry.Value;

		  formatter.formatElement(writer, element);

		  foreach (ValidationResult result in results)
		  {
			formatter.formatResult(writer, result);
		  }
		}
	  }

	  public virtual IDictionary<ModelElementInstance, IList<ValidationResult>> Results
	  {
		  get
		  {
			return collectedResults;
		  }
	  }

	}

}