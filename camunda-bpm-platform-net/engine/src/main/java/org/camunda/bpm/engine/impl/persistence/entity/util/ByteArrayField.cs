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
namespace org.camunda.bpm.engine.impl.persistence.entity.util
{
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ValueFields = org.camunda.bpm.engine.impl.variable.serializer.ValueFields;
	using ResourceType = org.camunda.bpm.engine.repository.ResourceType;

	/// <summary>
	/// A byte array value field what load and save <seealso cref="ByteArrayEntity"/>. It can
	/// be used in an entity which implements <seealso cref="ValueFields"/>.
	/// 
	/// @author Philipp Ossler
	/// </summary>
	public class ByteArrayField
	{

	  protected internal ByteArrayEntity byteArrayValue;
	  protected internal string byteArrayId;

	  protected internal readonly Nameable nameProvider;
	  protected internal ResourceType type;

	  protected internal string rootProcessInstanceId;
	  protected internal DateTime removalTime;

	  public ByteArrayField(Nameable nameProvider, ResourceType type, string rootProcessInstanceId, DateTime removalTime) : this(nameProvider, type)
	  {
		this.removalTime = removalTime;
		this.rootProcessInstanceId = rootProcessInstanceId;
	  }

	  public ByteArrayField(Nameable nameProvider, ResourceType type)
	  {
		this.nameProvider = nameProvider;
		this.type = type;
	  }

	  public virtual string ByteArrayId
	  {
		  get
		  {
			return byteArrayId;
		  }
		  set
		  {
			this.byteArrayId = value;
			this.byteArrayValue = null;
		  }
	  }


	  public virtual sbyte[] getByteArrayValue()
	  {
		ByteArrayEntity;

		if (byteArrayValue != null)
		{
		  return byteArrayValue.Bytes;
		}
		else
		{
		  return null;
		}
	  }

	  protected internal virtual ByteArrayEntity ByteArrayEntity
	  {
		  get
		  {
    
			if (byteArrayValue == null)
			{
			  if (!string.ReferenceEquals(byteArrayId, null))
			  {
				// no lazy fetching outside of command context
				if (Context.CommandContext != null)
				{
				  return byteArrayValue = Context.CommandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), byteArrayId);
				}
			  }
			}
    
			return byteArrayValue;
		  }
	  }

	  public virtual void setByteArrayValue(sbyte[] bytes)
	  {
		setByteArrayValue(bytes, false);
	  }

	  public virtual void setByteArrayValue(sbyte[] bytes, bool isTransient)
	  {
		if (bytes != null)
		{
		  // note: there can be cases where byteArrayId is not null
		  //   but the corresponding byte array entity has been removed in parallel;
		  //   thus we also need to check if the actual byte array entity still exists
		  if (!string.ReferenceEquals(this.byteArrayId, null) && ByteArrayEntity != null)
		  {
			byteArrayValue.Bytes = bytes;
		  }
		  else
		  {
			deleteByteArrayValue();

			byteArrayValue = new ByteArrayEntity(nameProvider.Name, bytes, type, rootProcessInstanceId, removalTime);

			// avoid insert of byte array value for a transient variable
			if (!isTransient)
			{
			  Context.CommandContext.ByteArrayManager.insertByteArray(byteArrayValue);

			  byteArrayId = byteArrayValue.Id;
			}
		  }
		}
		else
		{
		  deleteByteArrayValue();
		}

	  }

	  public virtual void deleteByteArrayValue()
	  {
		if (!string.ReferenceEquals(byteArrayId, null))
		{
		  // the next apparently useless line is probably to ensure consistency in the DbSqlSession cache,
		  // but should be checked and docked here (or removed if it turns out to be unnecessary)
		  ByteArrayEntity;

		  if (byteArrayValue != null)
		  {
			Context.CommandContext.DbEntityManager.delete(byteArrayValue);
		  }

		  byteArrayId = null;
		}
	  }

	  public virtual void setByteArrayValue(ByteArrayEntity byteArrayValue)
	  {
		this.byteArrayValue = byteArrayValue;
	  }

	  public virtual string RootProcessInstanceId
	  {
		  set
		  {
			this.rootProcessInstanceId = value;
		  }
	  }

	  public virtual DateTime RemovalTime
	  {
		  set
		  {
			this.removalTime = value;
		  }
	  }

	}
}