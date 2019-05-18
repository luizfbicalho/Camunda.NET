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
namespace org.camunda.bpm.model.xml.validation
{
	using Bird = org.camunda.bpm.model.xml.testmodel.instance.Bird;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class IllegalBirdValidator : ModelElementValidator<Bird>
	{

	  protected internal string nameOfBird;

	  public IllegalBirdValidator(string nameOfBird)
	  {
		this.nameOfBird = nameOfBird;
	  }

	  public virtual Type<Bird> ElementType
	  {
		  get
		  {
			return typeof(Bird);
		  }
	  }

	  public virtual void validate(Bird bird, ValidationResultCollector validationResultCollector)
	  {

		if (nameOfBird.Equals(bird.Id))
		{
		  validationResultCollector.addError(20, string.Format("Bird {0} is illegal", nameOfBird));
		}

	  }

	}

}