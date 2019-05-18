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
namespace org.camunda.bpm.model.xml.impl.validation
{

	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementValidator = org.camunda.bpm.model.xml.validation.ModelElementValidator;
	using ValidationResults = org.camunda.bpm.model.xml.validation.ValidationResults;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ModelInstanceValidator
	{

	  protected internal ModelInstanceImpl modelInstanceImpl;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Collection<org.camunda.bpm.model.xml.validation.ModelElementValidator<?>> validators;
	  protected internal ICollection<ModelElementValidator<object>> validators;

	  public ModelInstanceValidator<T1>(ModelInstanceImpl modelInstanceImpl, ICollection<T1> validators)
	  {
		this.modelInstanceImpl = modelInstanceImpl;
		this.validators = validators;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public org.camunda.bpm.model.xml.validation.ValidationResults validate()
	  public virtual ValidationResults validate()
	  {

		ValidationResultsCollectorImpl resultsCollector = new ValidationResultsCollectorImpl();

		foreach (ModelElementValidator validator in validators)
		{

		  Type elementType = validator.ElementType;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Collection<? extends org.camunda.bpm.model.xml.instance.ModelElementInstance> modelElementsByType = modelInstanceImpl.getModelElementsByType(elementType);
		  ICollection<ModelElementInstance> modelElementsByType = modelInstanceImpl.getModelElementsByType(elementType);

		  foreach (ModelElementInstance element in modelElementsByType)
		  {

			resultsCollector.CurrentElement = element;

			try
			{
			  validator.validate(element, resultsCollector);
			}
			catch (Exception e)
			{
			  throw new Exception("Validator " + validator + " threw an exception while validating " + element, e);
			}
		  }

		}

		return resultsCollector.Results;
	  }

	}

}