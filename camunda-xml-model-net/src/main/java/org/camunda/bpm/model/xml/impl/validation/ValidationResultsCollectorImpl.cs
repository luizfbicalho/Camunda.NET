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

	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ValidationResult = org.camunda.bpm.model.xml.validation.ValidationResult;
	using ValidationResultType = org.camunda.bpm.model.xml.validation.ValidationResultType;
	using ValidationResults = org.camunda.bpm.model.xml.validation.ValidationResults;
	using ValidationResultCollector = org.camunda.bpm.model.xml.validation.ValidationResultCollector;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ValidationResultsCollectorImpl : ValidationResultCollector
	{

	  protected internal ModelElementInstance currentElement;

	  protected internal IDictionary<ModelElementInstance, IList<ValidationResult>> collectedResults = new Dictionary<ModelElementInstance, IList<ValidationResult>>();

	  protected internal int errorCount = 0;
	  protected internal int warningCount = 0;

	  public virtual void addError(int code, string message)
	  {
		resultsForCurrentElement().Add(new ModelValidationResultImpl(currentElement, ValidationResultType.ERROR, code, message));

		++errorCount;
	  }

	  public virtual void addWarning(int code, string message)
	  {
		resultsForCurrentElement().Add(new ModelValidationResultImpl(currentElement, ValidationResultType.WARNING, code, message));

		++warningCount;
	  }

	  public virtual ModelElementInstance CurrentElement
	  {
		  set
		  {
			this.currentElement = value;
		  }
	  }

	  public virtual ValidationResults Results
	  {
		  get
		  {
			return new ModelValidationResultsImpl(collectedResults, errorCount, warningCount);
		  }
	  }

	  protected internal virtual IList<ValidationResult> resultsForCurrentElement()
	  {
		IList<ValidationResult> resultsByElement = collectedResults[currentElement];

		if (resultsByElement == null)
		{
		  resultsByElement = new List<ValidationResult>();
		  collectedResults[currentElement] = resultsByElement;
		}
		return resultsByElement;
	  }

	}

}