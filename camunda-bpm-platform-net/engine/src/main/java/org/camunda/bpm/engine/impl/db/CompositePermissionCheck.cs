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

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class CompositePermissionCheck
	{

	  protected internal bool disjunctive;

	  protected internal IList<CompositePermissionCheck> compositeChecks = new List<CompositePermissionCheck>();

	  protected internal IList<PermissionCheck> atomicChecks = new List<PermissionCheck>();

	  public CompositePermissionCheck() : this(true)
	  {
	  }

	  public CompositePermissionCheck(bool disjunctive)
	  {
		this.disjunctive = disjunctive;
	  }

	  public virtual void addAtomicCheck(PermissionCheck permissionCheck)
	  {
		this.atomicChecks.Add(permissionCheck);
	  }

	  public virtual IList<PermissionCheck> AtomicChecks
	  {
		  set
		  {
			this.atomicChecks = value;
		  }
		  get
		  {
			return atomicChecks;
		  }
	  }

	  public virtual IList<CompositePermissionCheck> CompositeChecks
	  {
		  set
		  {
			this.compositeChecks = value;
		  }
		  get
		  {
			return compositeChecks;
		  }
	  }

	  public virtual void addCompositeCheck(CompositePermissionCheck subCheck)
	  {
		this.compositeChecks.Add(subCheck);
	  }

	  /// <summary>
	  /// conjunctive else
	  /// </summary>
	  public virtual bool Disjunctive
	  {
		  get
		  {
			return disjunctive;
		  }
	  }



	  public virtual void clear()
	  {
		compositeChecks.Clear();
		atomicChecks.Clear();
	  }

	  public virtual IList<PermissionCheck> AllPermissionChecks
	  {
		  get
		  {
			IList<PermissionCheck> allChecks = new List<PermissionCheck>();
    
			((IList<PermissionCheck>)allChecks).AddRange(atomicChecks);
    
			foreach (CompositePermissionCheck compositePermissionCheck in compositeChecks)
			{
			  ((IList<PermissionCheck>)allChecks).AddRange(compositePermissionCheck.AllPermissionChecks);
			}
    
			return allChecks;
    
		  }
	  }

	}

}