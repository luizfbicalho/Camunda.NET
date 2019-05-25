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
namespace org.camunda.bpm.engine.impl.db
{

	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using AuthorizationManager = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class PermissionCheckBuilder
	{

	  protected internal IList<PermissionCheck> atomicChecks = new List<PermissionCheck>();
	  protected internal IList<CompositePermissionCheck> compositeChecks = new List<CompositePermissionCheck>();
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool disjunctive_Conflict = true;

	  protected internal PermissionCheckBuilder parent;

	  public PermissionCheckBuilder()
	  {
	  }

	  public PermissionCheckBuilder(PermissionCheckBuilder parent)
	  {
		this.parent = parent;
	  }

	  public virtual PermissionCheckBuilder disjunctive()
	  {
		this.disjunctive_Conflict = true;
		return this;
	  }

	  public virtual PermissionCheckBuilder conjunctive()
	  {
		this.disjunctive_Conflict = false;
		return this;
	  }

	  public virtual PermissionCheckBuilder atomicCheck(Resource resource, string queryParam, Permission permission)
	  {
		if (!isPermissionDisabled(permission))
		{
		  PermissionCheck permCheck = new PermissionCheck();
		  permCheck.Resource = resource;
		  permCheck.ResourceIdQueryParam = queryParam;
		  permCheck.Permission = permission;
		  this.atomicChecks.Add(permCheck);
		}

		return this;
	  }

	  public virtual PermissionCheckBuilder atomicCheckForResourceId(Resource resource, string resourceId, Permission permission)
	  {
		if (!isPermissionDisabled(permission))
		{
		  PermissionCheck permCheck = new PermissionCheck();
		  permCheck.Resource = resource;
		  permCheck.ResourceId = resourceId;
		  permCheck.Permission = permission;
		  this.atomicChecks.Add(permCheck);
		}

		return this;
	  }

	  public virtual PermissionCheckBuilder composite()
	  {
		return new PermissionCheckBuilder(this);
	  }

	  public virtual PermissionCheckBuilder done()
	  {
		parent.compositeChecks.Add(this.build());
		return parent;
	  }

	  public virtual CompositePermissionCheck build()
	  {
		validate();

		CompositePermissionCheck permissionCheck = new CompositePermissionCheck(disjunctive_Conflict);
		permissionCheck.AtomicChecks = atomicChecks;
		permissionCheck.CompositeChecks = compositeChecks;

		return permissionCheck;
	  }

	  public virtual IList<PermissionCheck> AtomicChecks
	  {
		  get
		  {
			return atomicChecks;
		  }
	  }

	  protected internal virtual void validate()
	  {
		if (atomicChecks.Count > 0 && compositeChecks.Count > 0)
		{
		  throw new ProcessEngineException("Mixed authorization checks of atomic and composite permissions are not supported");
		}
	  }

	  public virtual bool isPermissionDisabled(Permission permission)
	  {
		AuthorizationManager authorizationManager = Context.CommandContext.AuthorizationManager;
		return authorizationManager.isPermissionDisabled(permission);
	  }
	}

}