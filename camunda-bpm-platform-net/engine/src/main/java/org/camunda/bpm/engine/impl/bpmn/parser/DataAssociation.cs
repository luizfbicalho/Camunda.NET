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
namespace org.camunda.bpm.engine.impl.bpmn.parser
{
	using Expression = org.camunda.bpm.engine.@delegate.Expression;

	/// <summary>
	/// A data association (Input or Output) between a source and a target
	/// </summary>
	public class DataAssociation
	{

	  protected internal string source;

	  protected internal Expression sourceExpression;

	  protected internal string target;

	  protected internal string variables;

	  protected internal Expression businessKeyExpression;

	  protected internal DataAssociation(string source, string target)
	  {
		this.source = source;
		this.target = target;
	  }

	  protected internal DataAssociation(Expression sourceExpression, string target)
	  {
		this.sourceExpression = sourceExpression;
		this.target = target;
	  }

	  protected internal DataAssociation(string variables)
	  {
		this.variables = variables;
	  }

	  protected internal DataAssociation(Expression businessKeyExpression)
	  {
		this.businessKeyExpression = businessKeyExpression;
	  }

	  public virtual string Source
	  {
		  get
		  {
			return source;
		  }
	  }

	  public virtual string Target
	  {
		  get
		  {
			return target;
		  }
	  }


	  public virtual Expression SourceExpression
	  {
		  get
		  {
			return sourceExpression;
		  }
	  }

	  public virtual string Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  public virtual Expression BusinessKeyExpression
	  {
		  get
		  {
			return businessKeyExpression;
		  }
	  }
	}

}