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

	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using FlyingAnimal = org.camunda.bpm.model.xml.testmodel.instance.FlyingAnimal;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("resource") public class TestResultFormatter implements ValidationResultFormatter
	public class TestResultFormatter : ValidationResultFormatter
	{
		public virtual void formatElement(StringWriter writer, ModelElementInstance element)
		{
		Formatter formatter = new Formatter(writer);

		if (element is FlyingAnimal)
		{
		  formatter.format("%s\n", ((FlyingAnimal)element).Id);
		}
		else
		{
		  formatter.format("%s\n", element.ElementType.TypeName);
		}

		formatter.flush();
		}

	  public virtual void formatResult(StringWriter writer, ValidationResult result)
	  {
		(new Formatter(writer)).format("\t%s (%d): %s\n", result.Type, result.Code, result.Message).flush();
	  }

	}

}