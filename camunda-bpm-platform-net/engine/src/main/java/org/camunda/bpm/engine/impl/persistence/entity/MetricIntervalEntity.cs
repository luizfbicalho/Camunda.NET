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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using MetricIntervalValue = org.camunda.bpm.engine.management.MetricIntervalValue;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	[Serializable]
	public class MetricIntervalEntity : MetricIntervalValue, DbEntity
	{


	  protected internal DateTime timestamp;

	  protected internal string name;

	  protected internal string reporter;

	  protected internal long value;

	  public MetricIntervalEntity(DateTime timestamp, string name, string reporter)
	  {
		this.timestamp = timestamp;
		this.name = name;
		this.reporter = reporter;
	  }

	  /// <summary>
	  /// Ctor will be used by Mybatis
	  /// </summary>
	  /// <param name="timestamp"> </param>
	  /// <param name="name"> </param>
	  /// <param name="reporter"> </param>
	  public MetricIntervalEntity(long? timestamp, string name, string reporter)
	  {
		this.timestamp = new DateTime(timestamp);
		this.name = name;
		this.reporter = reporter;
	  }

	  public virtual DateTime getTimestamp()
	  {
		return timestamp;
	  }

	  public virtual void setTimestamp(DateTime timestamp)
	  {
		this.timestamp = timestamp;
	  }

	  public virtual void setTimestamp(long timestamp)
	  {
		this.timestamp = new DateTime(timestamp);
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


	  public virtual string Reporter
	  {
		  get
		  {
			return reporter;
		  }
		  set
		  {
			this.reporter = value;
		  }
	  }


	  public virtual long Value
	  {
		  get
		  {
			return value;
		  }
		  set
		  {
			this.value = value;
		  }
	  }


	  public virtual string Id
	  {
		  get
		  {
			return name + reporter + timestamp.ToString();
		  }
		  set
		  {
			throw new System.NotSupportedException("Not supported yet.");
		  }
	  }


	  public virtual object PersistentState
	  {
		  get
		  {
			return typeof(MetricIntervalEntity);
		  }
	  }

	  public override int GetHashCode()
	  {
		int hash = 7;
		hash = 67 * hash + (this.timestamp != null ? this.timestamp.GetHashCode() : 0);
		hash = 67 * hash + (!string.ReferenceEquals(this.name, null) ? this.name.GetHashCode() : 0);
		hash = 67 * hash + (!string.ReferenceEquals(this.reporter, null) ? this.reporter.GetHashCode() : 0);
		return hash;
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MetricIntervalEntity other = (MetricIntervalEntity) obj;
		MetricIntervalEntity other = (MetricIntervalEntity) obj;
		if ((string.ReferenceEquals(this.name, null)) ? (!string.ReferenceEquals(other.name, null)) :!this.name.Equals(other.name))
		{
		  return false;
		}
		if ((string.ReferenceEquals(this.reporter, null)) ? (!string.ReferenceEquals(other.reporter, null)) :!this.reporter.Equals(other.reporter))
		{
		  return false;
		}
		if (this.timestamp != other.timestamp && (this.timestamp == null || !this.timestamp.Equals(other.timestamp)))
		{
		  return false;
		}
		return true;
	  }

	}

}