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
namespace org.camunda.bpm.engine.impl.variable
{

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Obsolete, Serializable]
	public class VariableDeclaration
	{

	  private const long serialVersionUID = 1L;

	  protected internal string name;
	  protected internal string type;
	  protected internal string sourceVariableName;
	  protected internal Expression sourceExpression;
	  protected internal string destinationVariableName;
	  protected internal Expression destinationExpression;
	  protected internal string link;
	  protected internal Expression linkExpression;


	  public virtual void initialize(VariableScope innerScopeInstance, VariableScope outerScopeInstance)
	  {
		if (!string.ReferenceEquals(sourceVariableName, null))
		{
		  if (outerScopeInstance.hasVariable(sourceVariableName))
		  {
			object value = outerScopeInstance.getVariable(sourceVariableName);
			innerScopeInstance.setVariable(destinationVariableName, value);
		  }
		  else
		  {
			throw new ProcessEngineException("Couldn't create variable '" + destinationVariableName + "', since the source variable '" + sourceVariableName + "does not exist");
		  }
		}

		if (sourceExpression != null)
		{
		  object value = sourceExpression.getValue(outerScopeInstance);
		  innerScopeInstance.setVariable(destinationVariableName, value);
		}

		if (!string.ReferenceEquals(link, null))
		{
		  if (outerScopeInstance.hasVariable(sourceVariableName))
		  {
			object value = outerScopeInstance.getVariable(sourceVariableName);
			innerScopeInstance.setVariable(destinationVariableName, value);
		  }
		  else
		  {
			throw new ProcessEngineException("Couldn't create variable '" + destinationVariableName + "', since the source variable '" + sourceVariableName + "does not exist");
		  }
		}

		if (linkExpression != null)
		{
		  object value = sourceExpression.getValue(outerScopeInstance);
		  innerScopeInstance.setVariable(destinationVariableName, value);
		}

	  }

	  public virtual void destroy(VariableScope innerScopeInstance, VariableScope outerScopeInstance)
	  {

		if (!string.ReferenceEquals(destinationVariableName, null))
		{
		  if (innerScopeInstance.hasVariable(sourceVariableName))
		  {
			object value = innerScopeInstance.getVariable(sourceVariableName);
			outerScopeInstance.setVariable(destinationVariableName, value);
		  }
		  else
		  {
			throw new ProcessEngineException("Couldn't destroy variable " + sourceVariableName + ", since it does not exist");
		  }
		}

		if (destinationExpression != null)
		{
		  object value = destinationExpression.getValue(innerScopeInstance);
		  outerScopeInstance.setVariable(destinationVariableName, value);
		}

		if (!string.ReferenceEquals(link, null))
		{
		  if (innerScopeInstance.hasVariable(sourceVariableName))
		  {
			object value = innerScopeInstance.getVariable(sourceVariableName);
			outerScopeInstance.setVariable(destinationVariableName, value);
		  }
		  else
		  {
			throw new ProcessEngineException("Couldn't destroy variable " + sourceVariableName + ", since it does not exist");
		  }
		}

		if (linkExpression != null)
		{
		  object value = sourceExpression.getValue(innerScopeInstance);
		  outerScopeInstance.setVariable(destinationVariableName, value);
		}
	  }

	  public VariableDeclaration(string name, string type)
	  {
		this.name = name;
		this.type = type;
	  }

	  public override string ToString()
	  {
		return "VariableDeclaration[" + name + ":" + type + "]";
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }


	  public virtual string SourceVariableName
	  {
		  get
		  {
			return sourceVariableName;
		  }
		  set
		  {
			this.sourceVariableName = value;
		  }
	  }


	  public virtual Expression SourceExpression
	  {
		  get
		  {
			return sourceExpression;
		  }
		  set
		  {
			this.sourceExpression = value;
		  }
	  }


	  public virtual string DestinationVariableName
	  {
		  get
		  {
			return destinationVariableName;
		  }
		  set
		  {
			this.destinationVariableName = value;
		  }
	  }


	  public virtual Expression DestinationExpression
	  {
		  get
		  {
			return destinationExpression;
		  }
		  set
		  {
			this.destinationExpression = value;
		  }
	  }


	  public virtual string Link
	  {
		  get
		  {
			return link;
		  }
		  set
		  {
			this.link = value;
		  }
	  }


	  public virtual Expression LinkExpression
	  {
		  get
		  {
			return linkExpression;
		  }
		  set
		  {
			this.linkExpression = value;
		  }
	  }

	}

}