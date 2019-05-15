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
namespace org.camunda.bpm.engine.impl.variable.serializer.jpa
{

	/// <summary>
	/// Class containing meta-data about Entity-classes.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class EntityMetaData
	{

	  private bool isJPAEntity = false;
	  private Type entityClass;
	  private System.Reflection.MethodInfo idMethod;
	  private System.Reflection.FieldInfo idField;

	  public virtual bool JPAEntity
	  {
		  get
		  {
			return isJPAEntity;
		  }
		  set
		  {
			this.isJPAEntity = value;
		  }
	  }


	  public virtual Type EntityClass
	  {
		  get
		  {
			return entityClass;
		  }
		  set
		  {
			this.entityClass = value;
		  }
	  }


	  public virtual System.Reflection.MethodInfo IdMethod
	  {
		  get
		  {
			return idMethod;
		  }
		  set
		  {
			this.idMethod = value;
			value.Accessible = true;
		  }
	  }


	  public virtual System.Reflection.FieldInfo IdField
	  {
		  get
		  {
			return idField;
		  }
		  set
		  {
			this.idField = value;
			value.Accessible = true;
		  }
	  }


	  public virtual Type IdType
	  {
		  get
		  {
			Type idType = null;
			if (idField != null)
			{
			  idType = idField.Type;
			}
			else if (idMethod != null)
			{
			  idType = idMethod.ReturnType;
			}
			return idType;
		  }
	  }
	}

}