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
namespace org.camunda.bpm.engine.impl.db.entitymanager.operation
{

	/// <summary>
	/// A bulk operation
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DbBulkOperation : DbOperation
	{

	  public DbBulkOperation()
	  {
	  }

	  public DbBulkOperation(DbOperationType operationType, Type entityType, string statement, object parameter)
	  {
		this.operationType = operationType;
		this.entityType = entityType;
		this.statement = statement;
		this.parameter = parameter;
	  }

	  protected internal string statement;
	  protected internal object parameter;

	  public override void recycle()
	  {
		statement = null;
		parameter = null;
		base.recycle();
	  }

	  public override bool Failed
	  {
		  get
		  {
			return false;
		  }
	  }

	  public virtual object Parameter
	  {
		  get
		  {
			return parameter;
		  }
		  set
		  {
			this.parameter = value;
		  }
	  }


	  public virtual string Statement
	  {
		  get
		  {
			return statement;
		  }
		  set
		  {
			this.statement = value;
		  }
	  }


	  public override string ToString()
	  {
		return operationType + " " + statement + " " + parameter;
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((parameter == null) ? 0 : parameter.GetHashCode());
		result = prime * result + ((string.ReferenceEquals(statement, null)) ? 0 : statement.GetHashCode());
		return result;
	  }

	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
		DbBulkOperation other = (DbBulkOperation) obj;
		if (parameter == null)
		{
		  if (other.parameter != null)
		  {
			return false;
		  }
		}
		else if (!parameter.Equals(other.parameter))
		{
		  return false;
		}
		if (string.ReferenceEquals(statement, null))
		{
		  if (!string.ReferenceEquals(other.statement, null))
		  {
			return false;
		  }
		}
		else if (!statement.Equals(other.statement))
		{
		  return false;
		}
		return true;
	  }


	}

}